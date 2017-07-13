using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using System.Threading;
using LazuriteUI.Icons;

namespace CommonPlugin
{
    [OnlyExecute]
    [HumanFriendlyName("Ожидание (минут)")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [LazuriteIcon(Icon.TimerPause)]
    public class WaitActionMinutes : IAction
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

        public event ValueChangedDelegate ValueChanged;

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
            Thread.Sleep(TimeSpan.FromMinutes(val));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
