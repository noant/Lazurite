using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions.CoreActions;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Одиночный сценарий")]
    public class SingleActionScenario : ScenarioBase
    {
        public IAction TargetAction { get; set; }

        public override AbstractValueType ValueType
        {
            get
            {
                return TargetAction.ValueType;
            }
            set
            {
                ///
            }
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            TargetAction.SetValue(context, context.Input);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return new[] { TargetAction.GetType() };
        }

        public override string CalculateCurrentValue()
        {
            return TargetAction.GetValue(new ExecutionContext(string.Empty, new OutputChangedDelegates(), new CancellationToken()));
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
            if (this.TargetAction is ICoreAction)
            {
                ((ICoreAction)TargetAction)
                    .SetTargetScenario(repository.Scenarios.SingleOrDefault(x=>x.Id.Equals(((ICoreAction)TargetAction).TargetScenarioId)));
            }
        }
    }
}
