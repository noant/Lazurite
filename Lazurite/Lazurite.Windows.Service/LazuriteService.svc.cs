using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Visual;
using Lazurite.Windows.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace Lazurite.Windows.Service
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    public class LazuriteService : IServer
    {
        private ScenariosRepositoryBase _scenariosRepository;
        private UsersRepositoryBase _usersRepository;
        private VisualSettingsRepository _visualSettings;
        private WarningHandlerBase _warningHandler;
        private string _secretKey;

        public LazuriteService(string secretKey)
        {
            _secretKey = secretKey;
            _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
            _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
            _visualSettings = Singleton.Resolve<VisualSettingsRepository>();
            _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        }

        public LazuriteService() : this("secretKey1234567") { }

        private T Handle<T>(Func<T> function, [CallerMemberName] string memberName="")
        {
            try
            {
                _warningHandler.DebugFormat("[{0}] execution started", memberName);
                var result = function();
                if (result is Array)
                    _warningHandler.DebugFormat("[{0}] result: [{1}] items", memberName, ((Array)(object)result).Length);
                else if (result is IList)
                    _warningHandler.DebugFormat("[{0}] result: [{1}] items", memberName, ((IList)result).Count);
                return result;
            }
            catch (Exception e)
            {
                _warningHandler.ErrorFormat(e, "[{0}] execution error", memberName);
                throw e;
            }
            finally
            {
                _warningHandler.DebugFormat("[{0}] executed", memberName);
            }
        }

        private void Handle(Action action, [CallerMemberName] string memberName = "")
        {
            try
            {
                _warningHandler.DebugFormat("[{0}] execution started", memberName);
                action();
            }
            catch (Exception e)
            {
                _warningHandler.ErrorFormat(e, "[{0}] execution error", memberName);
                throw e;
            }
            finally
            {
                _warningHandler.DebugFormat("[{0}] executed", memberName);
            }
        }

        private UserBase GetCurrentUser()
        {
            var login = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            var user = _usersRepository.Users.SingleOrDefault(x => x.Login.Equals(login));
            if (user == null)
                ThrowUnauthorizedAccessException();
            return user;
        }

        private ScenarioBase GetScenarioWithPrivileges(string scenarioId)
        {
            var user = GetCurrentUser();

            var scenario = _scenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId));

            if (scenario == null)
                ThrowScenarioNotExistException(scenarioId);

            if (!scenario.CanExecute(user, ScenarioStartupSource.PublicUsage))
                ThrowUnauthorizedAccessException();

            return scenario;
        }

        private void ThrowUnauthorizedAccessException()
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        private void ThrowScenarioNotExistException(string scenarioId)
        {
            throw new InvalidOperationException("Scenario not exist: "+scenarioId);
        }

        private UserVisualSettings GetVisualSettings(UserBase user, string scenarioId)
        {
            return Handle(() => {
                var visualSettings = _visualSettings.VisualSettings
                    .SingleOrDefault(x => x is UserVisualSettings &&
                    x.UserId.Equals(user.Id) && x.ScenarioId.Equals(scenarioId));

                //if we can not found visualSettings then get visualSetting of SystemUser
                if (visualSettings == null)
                    visualSettings = _visualSettings.VisualSettings
                        .SingleOrDefault(x => x is UserVisualSettings &&
                        x.UserId.Equals(_usersRepository.SystemUser.Id) &&
                        x.ScenarioId.Equals(scenarioId));

                //if we can not found visualSettings of SystemUser then create new VisualSettings
                if (visualSettings == null)
                    visualSettings = new UserVisualSettings()
                    {
                        PositionX = 0,
                        PositionY = 0,
                        ScenarioId = scenarioId,
                        UserId = user.Id
                    };

                return visualSettings;
            });
        }

        public Encrypted<string> CalculateScenarioValue(Encrypted<string> scenarioId)
        {
            return Handle(() => new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).CalculateCurrentValue(), _secretKey));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void ExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle(() => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).Execute(value.Decrypt(_secretKey), new CancellationToken()));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle(() => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).ExecuteAsync(value.Decrypt(_secretKey)));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            Handle(() => GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).ExecuteAsyncParallel(value.Decrypt(_secretKey), new CancellationToken()));
        }

        public EncryptedList<ScenarioInfoLW> GetChangedScenarios(DateTime since)
        {
            return Handle(() =>
            {
                since = since.ToUniversalTime();
                var user = GetCurrentUser();
                return new EncryptedList<ScenarioInfoLW>(_scenariosRepository
                    .Scenarios
                    .Where(x => 
                        x.LastChange >= since && x.CanExecute(user, ScenarioStartupSource.PublicUsage)
                    )
                    .Select(x => new ScenarioInfoLW()
                    {
                        CurrentValue = x.CalculateCurrentValue(),
                        ScenarioId = x.Id
                    }), _secretKey);
            });
        }
        
        public Encrypted<ScenarioInfo> GetScenarioInfo(Encrypted<string> scenarioId)
        {
            return Handle(() =>
            {
                var user = GetCurrentUser();
                var scenario = GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey));

                return new Encrypted<ScenarioInfo>(new ScenarioInfo()
                {
                    CurrentValue = scenario.CalculateCurrentValue(),
                    ScenarioId = scenario.Id,
                    ValueType = scenario.ValueType,
                    VisualSettings = GetVisualSettings(user, scenario.Id)
                }, _secretKey);
            });
        }

        public EncryptedList<ScenarioInfo> GetScenariosInfo()
        {
            return Handle(() =>
            {
                var user = GetCurrentUser();
                var result = new EncryptedList<ScenarioInfo>(_scenariosRepository
                    .Scenarios
                    .Where(x => x.CanExecute(user, ScenarioStartupSource.PublicUsage))
                    .Select(x => new ScenarioInfo()
                    {
                        CurrentValue = x.CalculateCurrentValue(),
                        ScenarioId = x.Id,
                        ValueType = x.ValueType,
                        Name = x.Name,
                        VisualSettings = GetVisualSettings(user, x.Id)
                    }), _secretKey);

                return result;
            });
        }

        public Encrypted<string> GetScenarioValue(Encrypted<string> scenarioId)
        {
            return Handle(() => new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).GetCurrentValue(), _secretKey));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool IsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue)
        {
            return Handle(() =>
            {
                var decryptedLastKnown = lastKnownValue.Decrypt(_secretKey);
                return !GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey))
                    .CalculateCurrentValue()
                    .Equals(decryptedLastKnown);
            });
        }
        
        public void SaveVisualSettings(Encrypted<UserVisualSettings> visualSettings)
        {
            Handle(() =>
            {
                var decryptedVS = visualSettings.Decrypt(_secretKey);
                var user = GetCurrentUser();
                decryptedVS = new UserVisualSettings()
                {
                    AddictionalData = decryptedVS.AddictionalData,
                    PositionX = decryptedVS.PositionX,
                    PositionY = decryptedVS.PositionY,
                    ScenarioId = decryptedVS.ScenarioId,
                    UserId = user.Id
                };
                _visualSettings.Add(decryptedVS);
            });
        }
    }
}