using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
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
        public ActionHolder ActionHolder { get; set; } = new ActionHolder();

        public override ValueTypeBase ValueType
        {
            get
            {
                return ActionHolder.Action.ValueType;
            }
            set
            {
                ///
            }
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            ActionHolder.Action.SetValue(context, context.Input);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return new[] { ActionHolder.Action.GetType() };
        }

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            if (!ActionHolder.Action.IsSupportsEvent)
                base.CalculateCurrentValueAsync(callback);
            //return cached value, callback in not neccesary
            else callback(GetCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            //if action not send some info when value changed then calculate value
            if (!ActionHolder.Action.IsSupportsEvent)
                return ActionHolder.Action.GetValue(new ExecutionContext(this, string.Empty, new OutputChangedDelegates(), new CancellationToken()));
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

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            if (this.ActionHolder.Action is ICoreAction && repository != null)
            {
                ((ICoreAction)ActionHolder.Action)
                    .SetTargetScenario(repository.Scenarios.SingleOrDefault(x=>x.Id.Equals(((ICoreAction)ActionHolder).TargetScenarioId)));
            }
            ActionHolder.Action.Initialize();
            _currentValue = ActionHolder.Action.GetValue(null);
            this.ActionHolder.Action.ValueChanged += (action, value) => SetCurrentValueInternal(value);
            return true;
        }

        public override void AfterInitilize()
        {
            //first getting action value
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new[] { ActionHolder.Action };
        }
    }
}
