using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
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
                if (e is UnauthorizedAccessException
                    || e is InvalidOperationException
                    || e is DecryptException)
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw e;
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
                if (e is UnauthorizedAccessException 
                    || e is InvalidOperationException 
                    || e is DecryptException)
                {
                    WarningHandler.WarnFormat("[{0}] execution error. {1}", memberName, e.Message); //write only message
                    throw e;
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

        private ScenarioBase GetScenarioWithPrivileges(string scenarioId, UserBase user)
        {
            var scenario = ScenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId));

            if (scenario == null)
                ThrowScenarioNotExistException(scenarioId);

            if (!scenario.CanExecute(user, ScenarioStartupSource.Network))
                ThrowUnauthorizedAccessException();

            return scenario;
        }

        private void ThrowUnauthorizedAccessException()
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        private void ThrowScenarioNotExistException(string scenarioId)
        {
            throw new InvalidOperationException("Scenario not exist: " + scenarioId);
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
            return Handle((user) => new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user).CalculateCurrentValue(), _secretKey));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void ExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user).Execute(value.Decrypt(_secretKey), out string executionId));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user).ExecuteAsync(value.Decrypt(_secretKey), out string executionId));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle((user) => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user).ExecuteAsyncParallel(value.Decrypt(_secretKey), new CancellationToken()));
        }

        public EncryptedList<ScenarioInfoLW> GetChangedScenarios(DateTime since)
        {
            return Handle((user) =>
            {
                since = since.ToUniversalTime();
                return new EncryptedList<ScenarioInfoLW>(ScenariosRepository
                    .Scenarios
                    .Where(x => 
                        x.LastChange >= since && x.CanExecute(user, ScenarioStartupSource.Network)
                    )
                    .Select(x => new ScenarioInfoLW()
                    {
                        IsAvailable = x.IsAvailable,
                        CurrentValue = x.CalculateCurrentValue(),
                        ScenarioId = x.Id
                    }), _secretKey);
            });
        }
        
        public Encrypted<ScenarioInfo> GetScenarioInfo(Encrypted<string> scenarioId)
        {
            return Handle((user) =>
            {
                var scenario = GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user);

                return new Encrypted<ScenarioInfo>(new ScenarioInfo()
                {
                    CurrentValue = scenario.CalculateCurrentValue(),
                    ScenarioId = scenario.Id,
                    ValueType = scenario.ValueType,
                    VisualSettings = GetVisualSettings(user, scenario.Id),
                    Name = scenario.Name,
                    OnlyGetValue = scenario.OnlyGetValue,
                    IsAvailable = scenario.IsAvailable
                }, _secretKey);
            });
        }

        public EncryptedList<ScenarioInfo> GetScenariosInfo()
        {
            return Handle((user) =>
            {
                var result = new EncryptedList<ScenarioInfo>(ScenariosRepository
                    .Scenarios
                    .Where(x => x.CanExecute(user, ScenarioStartupSource.Network))
                    .Select(x => new ScenarioInfo()
                    {
                        CurrentValue = x.CalculateCurrentValue(),
                        ScenarioId = x.Id,
                        ValueType = x.ValueType,
                        Name = x.Name,
                        VisualSettings = GetVisualSettings(user, x.Id),
                        OnlyGetValue = x.OnlyGetValue,
                        IsAvailable = x.IsAvailable
                    }), 
                    _secretKey);

                return result;
            });
        }

        public Encrypted<string> GetScenarioValue(Encrypted<string> scenarioId)
        {
            return Handle((user) => new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user).GetCurrentValue(), _secretKey));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool IsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue)
        {
            return Handle((user) =>
            {
                var decryptedLastKnown = lastKnownValue.Decrypt(_secretKey);
                return !GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey), user)
                    .CalculateCurrentValue()
                    .Equals(decryptedLastKnown);
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
                var headers = OperationContext.Current.IncomingMessageHeaders;
                var data = encryptedData.Decrypt(_secretKey);
                data.Set(user); //crutch to identify current user in global data bus
                AddictionalDataManager.Handle(data);
                var location = data.Resolve<Geolocation>();
                if (location != null)
                    WarningHandler.InfoFormat("User [{0}] new geolocation: [];", user.Name, location);
                return new Encrypted<AddictionalData>(AddictionalDataManager.Prepare(), this._secretKey);
            });
        }
    }
}