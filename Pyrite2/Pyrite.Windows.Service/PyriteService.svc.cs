using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.Visual;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace Pyrite.Windows.Service
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    public class PyriteService : IServer
    {
        private ScenariosRepositoryBase _scenariosRepository;
        private UsersRepositoryBase _usersRepository;
        private VisualSettingsRepository _visualSettings;

        public PyriteService()
        {
#if DEBUG
            try
            {
#endif
                _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
                _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
                _visualSettings = Singleton.Resolve<VisualSettingsRepository>();
#if DEBUG
            }
            catch { }
#endif
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
                    ((UserVisualSettings)x).UserId.Equals(_usersRepository.SystemUser.Id) &&
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

        public string CalculateScenarioValue(string scenarioId)
        {
            return GetScenarioWithPrivileges(scenarioId).CalculateCurrentValue();
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void ExecuteScenario(string scenarioId, string value)
        {
            GetScenarioWithPrivileges(scenarioId).Execute(value, new CancellationToken());
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenario(string scenarioId, string value)
        {
            GetScenarioWithPrivileges(scenarioId).ExecuteAsync(value);
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void AsyncExecuteScenarioParallel(string scenarioId, string value)
        {
            GetScenarioWithPrivileges(scenarioId).ExecuteAsyncParallel(value, new CancellationToken());
        }

        public ScenarioInfoLW[] GetChangedScenarios(DateTime since)
        {
            var user = GetCurrentUser();
            return _scenariosRepository
                .Scenarios
                .Where(x => x.LastChange <= since && x.CanExecute(user, ScenarioStartupSource.Remote))
                .Select(x => new ScenarioInfoLW()
                {
                    CurrentValue = x.CalculateCurrentValue(),
                    ScenarioId = x.Id
                })
                .ToArray();
        }
        
        public ScenarioInfo GetScenarioInfo(string scenarioId)
        {
            var user = GetCurrentUser();
            var scenario = GetScenarioWithPrivileges(scenarioId);

            return new ScenarioInfo() {
                CurrentValue = scenario.CalculateCurrentValue(),
                ScenarioId = scenarioId,
                ValueType = scenario.ValueType,
                VisualSettings = GetVisualSettings(user, scenario.Id)
            };
        }

        public ScenarioInfo[] GetScenariosInfo()
        {
            var user = GetCurrentUser();
            return _scenariosRepository
                .Scenarios
                .Where(x => x.CanExecute(user, ScenarioStartupSource.Remote))
                .Select(x => new ScenarioInfo()
                {
                    CurrentValue = x.CalculateCurrentValue(),
                    ScenarioId = x.Id,
                    ValueType = x.ValueType,
                    VisualSettings = GetVisualSettings(user, x.Id)
                })
                .ToArray();
        }

        public string GetScenarioValue(string scenarioId)
        {
            return GetScenarioWithPrivileges(scenarioId).GetCurrentValue();
        }

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool IsScenarioValueChanged(string scenarioId, string lastKnownValue)
        {
            return GetScenarioWithPrivileges(scenarioId)
                .CalculateCurrentValue()
                .Equals(lastKnownValue);
        }
        
        public void SaveVisualSettings(UserVisualSettings visualSettings)
        {
            var user = GetCurrentUser();
            visualSettings = new UserVisualSettings() {
                Color = visualSettings.Color,
                PositionX = visualSettings.PositionX,
                PositionY = visualSettings.PositionY,
                ScenarioId = visualSettings.ScenarioId,
                UserId = user.Id
            };
            _visualSettings.Add(visualSettings);
        }
    }
}