using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [OnlyExecute]
    [HumanFriendlyName("Кнопка")]
    public class ButtonValueType : ValueTypeBase
    {
        public ButtonValueType() {
            _acceptedValues = new string[] { };
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
