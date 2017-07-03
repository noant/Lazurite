using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [OnlyGetValue]
    [VisualInitialization]
    [HumanFriendlyName("Стандартные: переключатель")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class GetToggleVTAction : IAction
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
                return ValueChanged != null;
            }
        }

        public string Value
        {
            get;
            set;
        }

        private ToggleValueType _valueType = new ToggleValueType();
        public ValueTypeBase ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = (ToggleValueType)value;
            }
        }

        public void Initialize()
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public string GetValue(ExecutionContext context)
        {
            return Value;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Value = value;
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
