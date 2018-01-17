using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Threading;

namespace CommonPlugin
{
    [OnlyExecute]
    [HumanFriendlyName("Ожидание (часов)")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [Category(Category.DateTime)]
    [LazuriteIcon(Icon.TimerPause)]
    public class WaitActionHours : IAction
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
        } = new FloatValueType() { AcceptedValues = new[] { "0", int.MaxValue.ToString() } };

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
        {
            return "0";
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
        {
            float val = 0;
            float.TryParse(value, out val);
            Thread.Sleep(TimeSpan.FromHours(val));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
