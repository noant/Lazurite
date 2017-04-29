using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Visual;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private string _secretKey;

        public LazuriteService(string secretKey)
        {
            _secretKey = secretKey;
            _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
            _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
            _visualSettings = Singleton.Resolve<VisualSettingsRepository>();
        }

        public LazuriteService() : this("secretKey1234567") { }

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
                ThrowScenarioNotExistException();

            if (!scenario.CanExecute(user, ScenarioStartupSource.Remote))
                ThrowUnauthorizedAccessException();

            return scenario;
        }

        private void ThrowUnauthorizedAccessException()
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        private void ThrowScenarioNotExistException()
        {
            throw new InvalidOperationException("Scenario not exist");
        }

        private UserVisualSettings GetVisualSettings(UserBase user, string scenarioId)
        {
            var visualSettings = _visualSettings.VisualSettings
                .SingleOrDefault(x => x is UserVisualSettings &&
                ((UserVisualSettings)x).UserId.Equals(user.Id) && x.ScenarioId.Equals(scenarioId));

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
                    Color = new byte[] { Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B },
                    PositionX = 0,
                    PositionY = 0,
                    ScenarioId = scenarioId,
                    UserId = user.Id
                };

            return visualSettings;
        }

        public Encrypted<string> CalculateScenarioValue(Encrypted<string> scenarioId)
        {
            return new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).CalculateCurrentValue(), _secretKey);
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void ExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).Execute(value.Decrypt(_secretKey), new CancellationToken());
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).ExecuteAsync(value.Decrypt(_secretKey));
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value)
        {
            GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).ExecuteAsyncParallel(value.Decrypt(_secretKey), new CancellationToken());
        }

        public EncryptedList<ScenarioInfoLW> GetChangedScenarios(DateTime since)
        {
            var user = GetCurrentUser();
            return new EncryptedList<ScenarioInfoLW>(_scenariosRepository
                .Scenarios
                .Where(x => x.LastChange <= since && x.CanExecute(user, ScenarioStartupSource.Remote))
                .Select(x => new ScenarioInfoLW()
                {
                    CurrentValue = x.CalculateCurrentValue(),
                    ScenarioId = x.Id
                }), _secretKey);
        }
        
        public Encrypted<ScenarioInfo> GetScenarioInfo(Encrypted<string> scenarioId)
        {
            var user = GetCurrentUser();
            var scenario = GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey));

            return new Encrypted<ScenarioInfo>(new ScenarioInfo() {
                CurrentValue = scenario.CalculateCurrentValue(),
                ScenarioId = scenario.Id,
                ValueType = scenario.ValueType,
                VisualSettings = GetVisualSettings(user, scenario.Id)
            }, _secretKey);
        }

        public EncryptedList<ScenarioInfo> GetScenariosInfo()
        {
            var user = GetCurrentUser();
            var result = new EncryptedList<ScenarioInfo>(_scenariosRepository
                .Scenarios
                .Where(x => x.CanExecute(user, ScenarioStartupSource.Remote))
                .Select(x => new ScenarioInfo()
                {
                    CurrentValue = x.CalculateCurrentValue(),
                    ScenarioId = x.Id,
                    ValueType = x.ValueType,
                    VisualSettings = GetVisualSettings(user, x.Id)
                }), _secretKey);

            return result;
        }

        public Encrypted<string> GetScenarioValue(Encrypted<string> scenarioId)
        {
            return new Encrypted<string>(GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey)).GetCurrentValue(), _secretKey);
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool IsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue)
        {
            var decryptedLastKnown = lastKnownValue.Decrypt(_secretKey);
            return GetScenarioWithPrivileges(scenarioId.Decrypt(_secretKey))
                .CalculateCurrentValue()
                .Equals(decryptedLastKnown);
        }
        
        public void SaveVisualSettings(Encrypted<UserVisualSettings> visualSettings)
        {
            var decryptedVS = visualSettings.Decrypt(_secretKey);
            var user = GetCurrentUser();
            decryptedVS = new UserVisualSettings() {
                Color = decryptedVS.Color,
                PositionX = decryptedVS.PositionX,
                PositionY = decryptedVS.PositionY,
                ScenarioId = decryptedVS.ScenarioId,
                UserId = user.Id
            };
            _visualSettings.Add(decryptedVS);
        }
    }
}