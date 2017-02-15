using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [OnlyExecute]
    [HumanFriendlyName("Кнопка")]
    public class ButtonValueType : ValueType
    {
        public ButtonValueType() {
            _acceptedValues = new[] { "PRESS" };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Кнопка";
            }
        }
    }
}
