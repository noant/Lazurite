using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    [HumanFriendlyName("Переключатель")]
    public class ToggleValueType: ValueType
    {
        public ToggleValueType()
        {
            _acceptedValues = new string[] { "ON", "OFF" };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Переключатель";
            }
        }
    }
}
