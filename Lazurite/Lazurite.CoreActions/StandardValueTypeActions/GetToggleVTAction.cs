using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [VisualInitialization]
    [HumanFriendlyName("# Переключатель")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class GetToggleVTAction : IAction, IStandardValueAction
    {
        public string Caption
        {
            get
            {
                return Value == ToggleValueType.ValueON ? "Включено" : "Выключено";
            }
            set
            {
                //
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public string Value
        {
            get;
            set;
        } = ToggleValueType.ValueOFF;
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ToggleValueType();

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public void Initialize()
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public string GetValue(ExecutionContext context)
        {
            return Value;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Value = value;
        }

        public event ValueChangedEventHandler ValueChanged;
    }
}
