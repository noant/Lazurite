using System.Linq;
using Lazurite.MainDomain;
using Lazurite.IOC;
using Lazurite.ActionsDomain;
using Lazurite.CoreActions.CoreActions;
using System;
using System.Threading;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions.CoreActions
{
    [HumanFriendlyName("Значение существующего сценария")]
    [VisualInitialization]
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    public class GetExistingScenarioValueAction : ICoreAction, IAction
    {
        public string TargetScenarioId
        {
            get; set;
        }

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

        private ScenarioBase _scenario;

        public event ValueChangedDelegate ValueChanged;

        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }
        
        public string Caption
        {
            get
            {
                return _scenario.Name;
            }
            set
            {
                //
            }
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return _scenario.ValueType;
            }
            set
            {
                //
            }
        }
        
        public void Initialize()
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public string GetValue(ExecutionContext context)
        {
            return _scenario.CalculateCurrentValue();
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }
    }
}