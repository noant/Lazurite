using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions
{
    [HumanFriendlyName("[действие не выбрано]")]
    public class EmptyAction : IAction
    {
        public string Caption
        {
            get
            {
                return string.Empty;
            }

            set
            {
                //do nothing
            }
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
                return false;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ButtonValueType();

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            //do nothing
            return false;
        }
    }
}
