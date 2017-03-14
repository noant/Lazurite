using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.CoreActions.CoreActions;
using Pyrite.IOC;

namespace Pyrite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase
    {
        public ComplexAction TargetAction { get; set; }

        public override ValueTypeBase ValueType
        {
            get;
            set;
        }

        public override string CalculateCurrentValue()
        {
            //just return last "returned" state
            return GetCurrentValue();
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            TargetAction.SetValue(context, string.Empty);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return TargetAction.GetAllActionsFlat().Select(x => x.GetType()).Distinct().ToArray();
        }

        private string _currentValue;
        public override void SetCurrentValueInternal(string value)
        {
            _currentValue = value;
            RaiseEvents();
        }

        public override string GetCurrentValue()
        {
            return _currentValue;
        }

        public override void Initialize(ScenariosRepositoryBase repository)
        {
            foreach (ICoreAction coreAction in this.TargetAction.GetAllActionsFlat().Where(x => x is ICoreAction))
                coreAction.SetTargetScenario(repository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
            ExecuteAsync(InitializeWithValue);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
        }

        public string InitializeWithValue { get; set; }
    }
}
