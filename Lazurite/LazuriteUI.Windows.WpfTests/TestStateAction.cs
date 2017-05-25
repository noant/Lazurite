using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;

namespace LazuriteUI.Windows.WpfTests
{
    public class TestStateAction : IAction
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
                return new StateValueType() {
                    AcceptedValues = new[] {
                        "Статус 1",
                        "Статус 2",
                        "Статус 3",
                        "Статус 4",
                        "Статус 5",
                        "Статус 6",
                        //"Статус 7",
                        //"Статус 8",
                        //"Статус 9",
                        //"Статус 11",
                        //"Статус 12",
                        //"Статус 13",
                        //"Статус 14",
                        //"Статус 15",
                        //"Статус 16",
                        //"Статус 17",
                        //"Статус 18",
                        //"Статус 19",
                        //"Статус 20",
                        //"Статус 30",
                        //"Статус 40",
                        //"Статус 50",
                        //"Статус 60",
                        //"Статус 70",
                        //"Статус 80",
                        //"Статус 90",
                    }
                };
            }

            set
            {

            }
        }

        public event ValueChangedDelegate ValueChanged;

        private string _val = "Статус 2";
        public string GetValue(ExecutionContext context)
        {
            return _val;
        }

        public void Initialize()
        {
        }

        public void SetValue(ExecutionContext context, string value)
        {
            _val = value;
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
