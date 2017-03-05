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
    [SuitableValueTypes(typeof(FloatValueType))]
    public class GetFloatVTAction : IAction
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

        private FloatValueType _valueType;
        public ActionsDomain.ValueTypes.AbstractValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = (FloatValueType)value;
            }
        }

        public void Initialize()
        {
            //
        }
        
        public void UserInitializeWith<T>() where T : AbstractValueType
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
    }
}
