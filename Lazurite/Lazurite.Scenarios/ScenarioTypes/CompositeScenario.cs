using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase, IStandardValueAction
    {
        public ComplexAction TargetAction { get; set; } = new ComplexAction();

        public override ValueTypeBase ValueType
        {
            get;
            set;
        }

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            //async is unneccesary
            callback(CalculateCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            //just return last "returned" state
            return GetCurrentValue();
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            SetCurrentValueInternal(context.Input);
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

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            foreach (var action in this.TargetAction.GetAllActionsFlat())
            {
                if (action != null)
                {
                    action.Initialize();
                    var coreAction = action as ICoreAction;
                    coreAction?.SetTargetScenario(repository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
                }
            }
            return true;
        }

        public override void AfterInitilize()
        {
            if (this.ValueType != null && this.ValueType is ButtonValueType == false) //except buttonValueType because any input value starts scenario permanent
                ExecuteAsync(InitializeWithValue);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
        }

        public string InitializeWithValue { get; set; }

        //crutch
        public string Value
        {
            get
            {
                return InitializeWithValue;
            }
            set
            {
                InitializeWithValue = value;
            }
        }
    }
}
