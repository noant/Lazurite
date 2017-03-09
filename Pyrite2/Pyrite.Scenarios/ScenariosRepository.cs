using Pyrite.CoreActions.CoreActions;
using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyrite.Scenarios
{
    public class ScenariosRepository: ScenariosRepositoryBase
    {
        private ISavior _savior;
        private List<string> _scenariosIds;
        private List<ScenarioBase> _scenarios;
        private readonly string _scenariosIdsKey = "scenariosRepository";

        public ScenariosRepository()
        {
            _savior = Singleton.Resolve<ISavior>();

            if (_savior.Has(_scenariosIdsKey))
                _scenariosIds = _savior.Get<List<string>>(_scenariosIdsKey);
            else _scenariosIds = new List<string>();

            _scenarios = _scenariosIds.Select(x => _savior.Get<ScenarioBase>(x)).ToList();

            //initialize scenarios
            foreach (var scenario in _scenarios)
                scenario.Initialize(this);
        }

        public override ScenarioBase[] Scenarios
        {
            get
            {
                return _scenarios.ToArray();
            }
        }

        public override void AddScenario(ScenarioBase scenario)
        {
            if (string.IsNullOrEmpty(scenario.Id))
                scenario.Id = Guid.NewGuid().ToString();
            if (_scenariosIds.Contains(scenario.Id))
                throw new InvalidOperationException("Scenario with same id already exist");
            _scenarios.Add(scenario);
            _scenariosIds.Add(scenario.Id);
            _savior.Set(scenario.Id, scenario);
            _savior.Set(_scenariosIdsKey, _scenariosIds);
        }

        public override void RemoveScenario(ScenarioBase scenario)
        {
            var linkedScenarios = _scenarios.Except(new[] { scenario })
                .Where(x => x.GetAllActionsFlat()
                    .Any(z => (z is ICoreAction) && ((ICoreAction)z).TargetScenarioId.Equals(scenario.Id))).ToArray();

            if (linkedScenarios.Any())
            {
                throw new InvalidOperationException("Cannot remove scenario, because other scenarios has reference on it: " 
                    + linkedScenarios.Select(x => x.Name).Aggregate((z, y) => z + "; " + y));
            }

            _scenariosIds.Remove(scenario.Id);
            _scenarios.RemoveAll(x => x.Id.Equals(scenario.Id));
            _savior.Set(_scenariosIdsKey, _scenariosIds);
            _savior.Clear(scenario.Id);
        }

        public override void SaveScenario(ScenarioBase scenario)
        {
            _savior.Set(scenario.Id, scenario);
        }
    }
}
