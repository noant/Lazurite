using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Одиночный сценарий")]
    public class SingleActionScenario : ScenarioBase
    {
        public IAction TargetAction { get; set; }

        public override ValueTypeBase ValueType
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

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            if (!TargetAction.IsSupportsEvent)
                base.CalculateCurrentValueAsync(callback);
            //return cached value, callback in not neccesary
            else callback(GetCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            //if action not send some info when value changed then calculate value
            if (!TargetAction.IsSupportsEvent)
                return TargetAction.GetValue(new ExecutionContext(string.Empty, new OutputChangedDelegates(), new CancellationToken()));
            //else - cached value is fresh
            return GetCurrentValue();
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
            if (this.TargetAction is ICoreAction && repository != null)
            {
                ((ICoreAction)TargetAction)
                    .SetTargetScenario(repository.Scenarios.SingleOrDefault(x=>x.Id.Equals(((ICoreAction)TargetAction).TargetScenarioId)));
            }
            TargetAction.Initialize();
            _currentValue = TargetAction.GetValue(null);
            this.TargetAction.ValueChanged += (action, value) => SetCurrentValueInternal(value);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new[] { TargetAction };
        }
    }
}
