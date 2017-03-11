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
    [InheritsValueTypeParams]
    [HumanFriendlyName("Статус")]
    [SuitableValueTypes(typeof(StateValueType))]
    public class GetStateVTAction : IAction
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

        private StateValueType _valueType;
        public AbstractValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = (StateValueType)value;
            }
        }

        public void Initialize()
        {
            //
        }
        
        public void UserInitializeWith(AbstractValueType valueType)
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
