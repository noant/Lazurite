using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain.ValueTypes;
using TestLib;

namespace Pyrite2TestModules
{
    public class TestAction1 : IAction
    {
        public string Caption
        {
            get
            {
                return "TestAction1";
            }
            set
            {
                //
            }
        }

        private DateTimeExt _currentValue = new DateTimeExt() { Value = DateTime.Now };
        public string Value
        {
            get
            {
                return _currentValue.Value.ToString();
            }
            set
            {
                _currentValue = new DateTimeExt() { Value = DateTime.Parse(value) };
            }
        }

        private DateTimeValueType _valueType = new DateTimeValueType();
        public AbstractValueType ValueType
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
        
        public void UserInitializeWith<T>() where T : AbstractValueType
        {
            //
        }
    }
}
