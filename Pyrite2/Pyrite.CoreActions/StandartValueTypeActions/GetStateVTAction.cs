using Pyrite.ActionsDomain;
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
    [HumanFriendlyName("Статус")]
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
        public ActionsDomain.ValueTypes.ValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }

        public void Initialize()
        {
            //
        }

        public void UserInitialize()
        {
            //
        }
    }
}
