using Lazurite.CoreActions.CoreActions;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lazurite.Scenarios
{
    public class ScenariosRepository: ScenariosRepositoryBase
    {
        private readonly string ScenariosIdsKey = "scenariosRepository";
        private readonly string TriggersIdsKey = "triggersRepository";

        private SaviorBase _savior;
        private List<string> _scenariosIds;
        private List<string> _triggersIds;
        private List<ScenarioBase> _scenarios;
        private List<TriggerBase> _triggers;
        private Logging.ILogger _log;

        public ScenariosRepository()
        {
            _savior = Singleton.Resolve<SaviorBase>();
            _log = Singleton.Resolve<ILogger>();
        }

        public override void Initialize()
        {
            if (_savior.Has(ScenariosIdsKey))
                _scenariosIds = _savior.Get<List<string>>(ScenariosIdsKey);
            else _scenariosIds = new List<string>();

            if (_savior.Has(TriggersIdsKey))
                _triggersIds = _savior.Get<List<string>>(TriggersIdsKey);
            else _triggersIds = new List<string>();

            _scenarios = _scenariosIds.Select(x => _savior.Get<ScenarioBase>(x)).ToList();
            _triggers = _triggersIds.Select(x => _savior.Get<TriggerBase>(x)).ToList();

            //initialize scenarios
            Task.WhenAll(
                _scenarios
                .Where(x => x.GetInitializationState() == ScenarioInitializationValue.NotInitialized)
                .Select(x => x.FullInitialize()));

            //initialize triggers
            foreach (var trigger in _triggers)
            {
                trigger.Initialize();
                trigger.AfterInitialize();
            }
        }

        public override ScenarioBase[] Scenarios => _scenarios.ToArray();

        public override TriggerBase[] Triggers => _triggers.ToArray();

        public override void AddScenario(ScenarioBase scenario)
        {
            if (_scenariosIds.Contains(scenario.Id))
                throw new InvalidOperationException("Scenario with same id already exist");
            _scenarios.Add(scenario);
            _scenariosIds.Add(scenario.Id);
            _savior.Set(scenario.Id, scenario);
            _savior.Set(ScenariosIdsKey, _scenariosIds);
        }

        public override void RemoveScenario(ScenarioBase scenario)
        {
            var linkedScenarios = _scenarios.Except(new[] { scenario })
                                    .Where(x => x.GetAllActionsFlat()
                                        .Any(z => z is IScenariosAccess scenarioAccessObj && scenarioAccessObj.TargetScenarioId != null && scenarioAccessObj.TargetScenarioId.Equals(scenario.Id))).ToArray();

            if (linkedScenarios.Any())
            {
                throw new InvalidOperationException("Невозможно удалить сценарий, так как следующие сценарии ссылаются на него: " 
                    + linkedScenarios.Select(x => x.Name).Aggregate((z, y) => z + "; " + y));
            }

            var linkedTriggers = _triggers
                                    .Where(x => (x.TargetScenarioId != null && x.TargetScenarioId.Equals(scenario.Id)) || x.GetAllActionsFlat()
                                        .Any(z => z is IScenariosAccess scenarioAccessObj && scenarioAccessObj.TargetScenarioId.Equals(scenario.Id))).ToArray();

            if (linkedScenarios.Any())
            {
                throw new InvalidOperationException("Невозможно удалить сценарий, так как следующие триггеры ссылаются на него: "
                    + linkedTriggers.Select(x => x.Name).Aggregate((z, y) => z + "; " + y));
            }

            scenario.Dispose();
            _scenariosIds.Remove(scenario.Id);
            _scenarios.RemoveAll(x => x.Id.Equals(scenario.Id));
            RaiseOnScenarioRemoved(scenario);
            _savior.Set(ScenariosIdsKey, _scenariosIds);
            _savior.Clear(scenario.Id);
        }

        public override void SaveScenario(ScenarioBase scenario)
        {
            _savior.Set(scenario.Id, scenario);
            var index = _scenarios.IndexOf(_scenarios.FirstOrDefault(x => x.Id.Equals(scenario.Id)));
            var prevScenario = _scenarios.FirstOrDefault(x => x.Id.Equals(scenario.Id));
            prevScenario?.Dispose();
            _scenarios.RemoveAll(x => x.Id.Equals(scenario.Id));
            _scenarios.Insert(index, scenario);

            var allActionsWithScen = 
                _scenarios
                .SelectMany(x => x.GetAllActionsFlat())
                .Union(_triggers.SelectMany(x => x.GetAllActionsFlat()))
                .Where(x => x is IScenariosAccess scenarioAccessObj && scenarioAccessObj.TargetScenarioId == scenario.Id)
                .ToArray();

            foreach (IScenariosAccess action in allActionsWithScen)
                action.SetTargetScenario(scenario);

            foreach (TriggerBase trigger in _triggers.Where(x => x.TargetScenarioId?.Equals(scenario.Id) ?? false))
            {
                trigger.SetScenario(scenario);
                if (trigger.Enabled)
                {
                    trigger.Stop();
                    trigger.Run();
                }
            }
        }

        public override void AddTrigger(TriggerBase trigger)
        {
            if (_triggersIds.Contains(trigger.Id))
                throw new InvalidOperationException("Trigger with same id already exist");
            _triggers.Add(trigger);
            _triggersIds.Add(trigger.Id);
            _savior.Set(TriggersIdsKey, _triggersIds);
            _savior.Set(trigger.Id, trigger);
        }

        public override void RemoveTrigger(TriggerBase trigger)
        {
            trigger.Stop();
            _triggersIds.Remove(trigger.Id);
            _triggers.RemoveAll(x => x.Id.Equals(trigger.Id));
            _savior.Set(TriggersIdsKey, _triggersIds);
            _savior.Clear(trigger.Id);
        }

        public override void SaveTrigger(TriggerBase trigger)
        {
            var prevTrigger = _triggers.FirstOrDefault(x => x.Id.Equals(trigger.Id));
            _savior.Set(trigger.Id, trigger);
            var index = _triggers.IndexOf(_triggers.FirstOrDefault(x => x.Id.Equals(trigger.Id)));
            _triggers.RemoveAll(x => x.Id.Equals(trigger.Id));
            _triggers.Insert(index, trigger);
        }

        public override void Dispose()
        {
            base.Dispose();
            _triggers = null;
            _scenarios = null;
        }
    }
}
