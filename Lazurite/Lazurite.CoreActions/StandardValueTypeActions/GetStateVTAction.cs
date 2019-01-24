using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [VisualInitialization]
    [HumanFriendlyName("# Статус")]
    [SuitableValueTypes(typeof(StateValueType))]
    [Category(Category.Meta)]
    public class GetStateVTAction : IAction, IStandardValueAction
    {
        public string Caption
        {
            get
            {
                return Value;
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
        } = string.Empty;
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new StateValueType();

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

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
    }
}
