using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public abstract class ScenariosRepositoryBase
    {
        public abstract ScenarioBase[] Scenarios { get; }
        public ScenarioBase[] GetDependentScenarios(Type[] types)
        {
            return Scenarios
                .Where(x => x.GetAllUsedActionTypes()
                .Any(z=>types.Any(y=>y.Equals(z)))).ToArray();
        }
        public ScenarioBase[] GetScenarios(UserBase user, ScenarioStartupSource source, AbstractValueType valueType=null, bool rightPart=false)
        {
            return Scenarios.Where(x =>
                x.CanExecute(user, source)
                && (valueType == null || valueType.IsCompatibleWith(x.ValueType))
                && (!rightPart || !(x.ValueType is ButtonValueType))).ToArray();
        }

        public abstract void AddScenario(ScenarioBase scenario);
        public abstract void RemoveScenario(ScenarioBase scenario);
        public abstract void SaveScenario(ScenarioBase scenario);

        protected void RaiseOnScenarioRemoved(ScenarioBase scenario)
        {
            OnScenarioRemoved?.Invoke(scenario);
        }

        public Action<ScenarioBase> OnScenarioRemoved { get; set; }
    }
}
