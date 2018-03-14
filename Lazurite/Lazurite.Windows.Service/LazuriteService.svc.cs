using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Shared;
using Lazurite.Visual;
using Lazurite.Windows.Logging;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;

namespace Lazurite.Windows.Service
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    public class LazuriteService : IServer
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly VisualSettingsRepository VisualSettings = Singleton.Resolve<VisualSettingsRepository>();
        private static readonly WarningHandlerBase WarningHandler = Singleton.Resolve<WarningHandlerBase>();
        private static readonly AddictionalDataManager AddictionalDataManager = Singleton.Resolve<AddictionalDataManager>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private string _secretKey;

        public LazuriteService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public LazuriteService() : this("secretKey1234567") { }

        private T Handle<T>(Func<UserBase,T> function, [CallerMemberName] string memberName="")
        {
            try
            {
                WarningHandler.DebugFormat("[{0}] execution started", memberName);
                var currentUser = GetCurrentUser();
                var result = function(currentUser);
                if (result is Array)
                    WarningHandler.DebugFormat("[{0}] result: [{1}] items", memberName, ((Array)(object)result).Length);
                else if (result is IList)
                    WarningHandler.DebugFormat("[{0}] result: [{1}] items", memberName, ((IList)result).Count);
                return result;
            }
            catch (Exception e)
            {
                if (SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.AccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectAccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectNotFound))
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw e;
                }
                else if (e is DecryptException)
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw new FaultException(e.Message, new FaultCode(ServiceFaultCodes.DecryptionError));
                }
                else if (e is ScenarioExecutionException executionError)
                {
                    if (executionError.ErrorType == ScenarioExecutionError.AccessDenied)
                        ThrowUnauthorizedAccessException();
                }
                else
                    WarningHandler.ErrorFormat(e, "[{0}] execution error", memberName); //unrecognized exception; write fully
            }
            finally
            {
                WarningHandler.DebugFormat("[{0}] executed", memberName);
            }
            return default(T);
        }

        private void Handle(Action<UserBase> action, [CallerMemberName] string memberName = "")
        {
            try
            {
                WarningHandler.DebugFormat("[{0}] execution started", memberName);
                var currentUser = GetCurrentUser();
                action(currentUser);
            }
            catch (Exception e)
            {
                if (SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.AccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectAccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectNotFound))
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw e;
                }
                else if (e is DecryptException)
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw new FaultException(e.Message, new FaultCode(ServiceFaultCodes.DecryptionError));
                }
                else if (e is ScenarioExecutionException executionError)
                {
                    if (executionError.ErrorType == ScenarioExecutionError.AccessDenied)
                        ThrowUnauthorizedAccessException();
                }
                else
                    WarningHandler.ErrorFormat(e, "[{0}] execution error", memberName); //unrecognized exception; write fully
            }
            finally
            {
                WarningHandler.DebugFormat("[{0}] executed", memberName);
            }
        }

        private UserBase GetCurrentUser()
        {
            var login = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(login));
            if (user == null)
                ThrowUnauthorizedAccessException();
            return user;
        }

        private ScenarioBase GetScenarioWithPrivileges(string scenarioId, ScenarioActionSource actionSource)
        {
            var scenario = GetScenario(scenarioId);
                        
            if (!scenario.IsAccessAvailable(actionSource))
                ThrowUnauthorizedAccessException();

            return scenario;
        }

        private ScenarioBase GetScenario(string scenarioId)
        {
            var scenario = ScenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId));

            if (scenario == null)
                ThrowScenarioNotExistException(scenarioId);

            return scenario;
        }

        private void ThrowUnauthorizedAccessException()
        {
            throw new FaultException("Access denied", new FaultCode(ServiceFaultCodes.ObjectAccessDenied));
        }

        private void ThrowScenarioNotExistException(string scenarioId)
        {
            throw new FaultException("Scenario not exist: " + scenarioId, new FaultCode(ServiceFaultCodes.ObjectNotFound));
        }

        private UserVisualSettings GetVisualSettings(UserBase user, string scenarioId)
        {
            var visualSettings = VisualSettings.VisualSettings
                .SingleOrDefault(x => x is UserVisualSettings &&
                x.UserId.Equals(user.Id) && x.ScenarioId.Equals(scenarioId));

            //if we can not found visualSettings then get visualSetting of SystemUser
            if (visualSettings == null)
                visualSettings = VisualSettings.VisualSettings
                    .SingleOrDefault(x => x is UserVisualSettings &&
                    x.UserId.Equals(UsersRepository.SystemUser.Id) &&
                    x.ScenarioId.Equals(scenarioId));
            
            return visualSettings;
        }

        public Encrypted<string> CalculateScenarioValue(Encrypted<string> scenarioId)
        {
            return Handle((user) =>
            {
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
                return new Encrypted<string>(scenario.CalculateCurrentValue(actionSource, null), _secretKey);
            });
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void ExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) =>
            {
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
                scenario.Execute(actionSource, value.Decrypt(_secretKey), out string executionId);
            });
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) =>
            {
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
                scenario.ExecuteAsync(actionSource, value.Decrypt(_secretKey), out string executionId);
            });
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) =>
            {
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
                scenario.ExecuteAsyncParallel(actionSource, value.Decrypt(_secretKey), null);
            });
        }

        public EncryptedList<ScenarioInfoLW> GetChangedScenarios(DateTime since)
        {
            return Handle((user) =>
            {
                since = since.ToUniversalTime();
                var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
                return new EncryptedList<ScenarioInfoLW>(ScenariosRepository
                    .Scenarios
                    .Where(x => x.LastChange >= since)
                    .Select(x =>
                    {
                        bool isAvailable = true;
                        var curVal = string.Empty;
                        try
                        {
                            curVal = x.CalculateCurrentValue(scenarioActionSource, null);
                        }
                        catch
                        {
                            isAvailable = false;
                            curVal = x.ValueType.DefaultValue;
                        }
                        return new ScenarioInfoLW()
                        {
                            IsAvailable = isAvailable && x.GetIsAvailable(),
                            CurrentValue = curVal,
                            ScenarioId = x.Id
                        };
                    }), _secretKey);
            });
        }
        
        public Encrypted<ScenarioInfo> GetScenarioInfo(Encrypted<string> scenarioId)
        {
            return Handle((user) =>
            {
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                var executeScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
                var viewScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);

                var currentValue = string.Empty;
                var canSetValue = true;
                var isAvailable = true;
                try
                {
                    currentValue = scenario.CalculateCurrentValue(viewScenarioAction, null);
                    canSetValue = scenario.IsAccessAvailable(executeScenarioAction);
                }
                catch
                {
                    isAvailable = false;
                    canSetValue = false;
                    currentValue = scenario.ValueType.DefaultValue;
                }

                return new Encrypted<ScenarioInfo>(new ScenarioInfo()
                {
                    CurrentValue = currentValue,
                    ScenarioId = scenario.Id,
                    ValueType = scenario.ValueType,
                    VisualSettings = GetVisualSettings(user, scenario.Id),
                    Name = scenario.Name,
                    OnlyGetValue = !canSetValue,
                    IsAvailable = isAvailable && scenario.GetIsAvailable()
                }, _secretKey);
            });
        }

        public EncryptedList<ScenarioInfo> GetScenariosInfo()
        {
            return Handle((user) =>
            {
                var executeScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
                var viewScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
                var result = new EncryptedList<ScenarioInfo>(ScenariosRepository
                    .Scenarios
                    .Where(x => x.IsAccessAvailable(viewScenarioAction))
                    .Select(x => new ScenarioInfo()
                    {
                        CurrentValue = x.CalculateCurrentValue(viewScenarioAction, null),
                        ScenarioId = x.Id,
                        ValueType = x.ValueType,
                        Name = x.Name,
                        VisualSettings = GetVisualSettings(user, x.Id),
                        OnlyGetValue = x.IsAccessAvailable(executeScenarioAction),
                        IsAvailable = x.GetIsAvailable()
                    }), 
                    _secretKey);

                return result;
            });
        }

        public Encrypted<string> GetScenarioValue(Encrypted<string> scenarioId)
        {
            return Handle((user) => {
                var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
                return new Encrypted<string>(GetScenario(scenarioId.Decrypt(_secretKey)).CalculateCurrentValue(scenarioActionSource, null), _secretKey);
            });
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool IsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue)
        {
            return Handle((user) =>
            {
                var decryptedLastKnown = lastKnownValue.Decrypt(_secretKey);
                var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
                var scenario = GetScenario(scenarioId.Decrypt(_secretKey));
                try
                {
                    return scenario
                        .CalculateCurrentValue(scenarioActionSource, null)
                        .Equals(decryptedLastKnown);
                }
                catch
                {
                    return false;
                }
            });
        }
        
        public void SaveVisualSettings(Encrypted<UserVisualSettings> visualSettings)
        {
            Handle((user) =>
            {
                var decryptedVS = visualSettings.Decrypt(_secretKey);
                decryptedVS = new UserVisualSettings()
                {
                    AddictionalData = decryptedVS.AddictionalData,
                    VisualIndex = decryptedVS.VisualIndex,
                    ScenarioId = decryptedVS.ScenarioId,
                    UserId = user.Id
                };
                VisualSettings.Add(decryptedVS);
                VisualSettings.Save();
            });
        }

        public Encrypted<AddictionalData> SyncAddictionalData(Encrypted<AddictionalData> encryptedData)
        {
            return Handle((user) => {
                WarningHandler.InfoFormat("User AddictionalData sync: [{0}];", user.Name);
                var data = encryptedData.Decrypt(_secretKey);
                AddictionalDataManager.Handle(data, user);
                var preparedData = AddictionalDataManager.Prepare(user);
                return new Encrypted<AddictionalData>(preparedData, _secretKey);
            });
        }
    }
}