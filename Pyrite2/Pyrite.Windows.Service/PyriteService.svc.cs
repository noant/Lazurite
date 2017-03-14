using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.Visual;
using System;
using System.Collections.Generic;
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
    public class PyriteService : IServer
    {
        private ScenariosRepositoryBase _scenariosRepository;
        private UsersRepositoryBase _usersRepository;
        private VisualSettingsRepository _visualSettings;

        public PyriteService()
        {
            try
            {
                _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
                _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
                _visualSettings = Singleton.Resolve<VisualSettingsRepository>();
            }
            catch { }
        }

        private UserBase GetCurrentUser()
        {
            var login = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            return _usersRepository.Users.SingleOrDefault(x => x.Login.Equals(login));
        }

        public string CalculateScenarioValue(string scenarioId)
        {
            return OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            //return _scenariosRepository
            //    .Scenarios
            //    .SingleOrDefault(x => x.Id.Equals(scenarioId))?
            //    .CalculateCurrentValue();
        }

        public void ExecuteScenario(string scenarioId, string value)
        {
            _scenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId))?
                .Execute(value, new CancellationToken());
        }

        public void AsyncExecuteScenario(string scenarioId, string value)
        {
            _scenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId))?
                .ExecuteAsync(value);
        }

        public void AsyncExecuteScenarioParallel(string scenarioId, string value)
        {
            _scenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId))?
                .ExecuteAsyncParallel(value, new CancellationToken());
        }

        public ScenarioInfoLW[] GetChangedScenarios(DateTime since)
        {
            return _scenariosRepository
                .Scenarios
                .Where(x => x.LastChange <= since)
                .Select(x => new ScenarioInfoLW()
                {
                    CurrentValue = x.CalculateCurrentValue(),
                    ScenarioId = x.Id
                })
                .ToArray();
        }

        public ScenarioInfo GetScenarioInfo(string scenarioId)
        {
            //var user = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.

            //var scenario = _scenariosRepository.Scenarios
            //    .SingleOrDefault(x => x.Id.Equals(scenarioId));

            //if (scenarioId == null)
            //    return null;

            //var currentScenarioVisualSettings = _visualSettings
            //    .VisualSettings
            //    .SingleOrDefault(x=> (x is UserVisualSettings) && ((UserVisualSettings)x).UserId.Equals()

            //return new ScenarioInfo()
            //{
            //    CurrentValue = scenario.CalculateCurrentValue(),
            //    ScenarioId = scenarioId,
            //    ValueType = scenario.ValueType,
            //    VisualSettings
            //}

            throw new NotImplementedException();
        }

        public ScenarioInfo[] GetScenariosInfo()
        {
            throw new NotImplementedException();
        }

        public string GetScenarioValue(string scenarioId)
        {
            throw new NotImplementedException();
        }

        public bool IsScenarioValueChanged(string scenarioId, string lastKnownValue)
        {
            throw new NotImplementedException();
        }

    }
}
