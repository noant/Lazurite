using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.StandartValueTypeActions
{
    [OnlyGetValue]
    [VisualInitialization]
    [HumanFriendlyName("Переключатель")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [InheritsValueTypeParams]
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
                //
            }
        }

        public void Initialize()
        {
            //
        }
        
        public void UserInitializeWith(ValueTypeBase valueType)
        {
            //
        }

        public string GetValue(ExecutionContext context)
        {
            return Value;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Value = value;
        }

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}
