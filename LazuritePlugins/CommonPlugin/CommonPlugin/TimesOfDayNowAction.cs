using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPlugin
{
    [OnlyGetValue]
    [HumanFriendlyName("Время дня сейчас")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.Timer)]
    public class TimesOfDayNowAction : IAction
    {
        public string Caption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueTypeBase ValueType { get; } = new StateValueType() {
            AcceptedValues = new {};
        };

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SetValue(ExecutionContext context, string value)
        {
            throw new NotImplementedException();
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            throw new NotImplementedException();
        }
    }
}
