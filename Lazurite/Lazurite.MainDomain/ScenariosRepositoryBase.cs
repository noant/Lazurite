using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using System;
using System.Linq;

namespace Lazurite.MainDomain
{
    public abstract class ScenariosRepositoryBase: IDisposable
    {
        public abstract ScenarioBase[] Scenarios { get; }
        public abstract TriggerBase[] Triggers { get; }
        
        public TriggerBase[] GetDependentTriggers(Type[] types)
        {
            return Triggers
                .Where(x => x.GetAllUsedActionTypes()
                .Any(z => types.Any(y => y.Equals(z)))).ToArray();
        }

        public ScenarioBase[] GetDependentScenarios(Type[] types)
        {
            return Scenarios
                .Where(x => x.GetAllUsedActionTypes()
                .Any(z=>types.Any(y=>y.Equals(z)))).ToArray();
        }

        public ScenarioBase[] GetScenarios(UserBase user, ScenarioStartupSource source, ScenarioAction action, ValueTypeBase valueType = null, bool rightPart = false)
        {
            var actionSource = new ScenarioActionSource(user, source, ScenarioAction.ViewValue);
            return Scenarios.Where(x =>
                (valueType == null || valueType.IsCompatibleWith(x.ValueType)) &&
                x.IsAccessAvailable(actionSource) &&
                (rightPart ? !(x.ValueType is ButtonValueType) : true))
                .ToArray();
        }

        public abstract void AddScenario(ScenarioBase scenario);
        public abstract void RemoveScenario(ScenarioBase scenario);
        public abstract void SaveScenario(ScenarioBase scenario);

        public abstract void AddTrigger(TriggerBase trigger);
        public abstract void RemoveTrigger(TriggerBase trigger);
        public abstract void SaveTrigger(TriggerBase trigger);

        protected void RaiseOnScenarioRemoved(ScenarioBase scenario)
        {
            OnScenarioRemoved?.Invoke(this, new EventsArgs<ScenarioBase>(scenario));
        }

        public void Dispose()
        {
            foreach (var trigger in Triggers)
                trigger.Stop();
            foreach (var scenario in Scenarios)
                scenario.Dispose();
        }

        public event EventsHandler<ScenarioBase> OnScenarioRemoved;

        public abstract void Initialize();
    }
}
