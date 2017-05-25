using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;

namespace LazuriteUI.Windows.WpfTests
{
    public class TestInfoAction : IAction
    {
        public string Caption
        {
            get;

            set;
        }

        public bool IsSupportsEvent
        {
            get;
            set;
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return new InfoValueType();
            }

            set
            {

            }
        }

        public event ValueChangedDelegate ValueChanged;

        private string _val = "Выключено";
        public string GetValue(ExecutionContext context)
        {
            return _val;
        }

        public void Initialize()
        {
        }

        public void SetValue(ExecutionContext context, string value)
        {
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
