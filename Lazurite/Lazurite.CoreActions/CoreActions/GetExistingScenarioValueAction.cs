using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;

namespace Lazurite.CoreActions.CoreActions
{
    [HumanFriendlyName("Значение существующего сценария")]
    [VisualInitialization]
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    public class GetExistingScenarioValueAction : IScenariosAccess, IAction
    {
        public string TargetScenarioId
        {
            get; set;
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        private ScenarioBase _scenario;

        public event ValueChangedEventHandler ValueChanged;

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
                return _scenario?.Name ?? "[сценарий не выбран]";
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
                return _scenario?.ValueType ?? new ButtonValueType();
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