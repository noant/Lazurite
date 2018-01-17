using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;

namespace CommonPlugin
{
    [OnlyGetValue]
    [HumanFriendlyName("Час сейчас")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [Category(Category.DateTime)]
    [LazuriteIcon(Icon.Timer)]
    public class HourNowAction : IAction
    {
        public string Caption
        {
            get;
            set;
        } = string.Empty;

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
        } = new FloatValueType() { AcceptedValues = new[] { "0", "23" } };

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return DateTime.Now.Hour.ToString();
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
            return true;
            //do nothing
        }
    }
}
