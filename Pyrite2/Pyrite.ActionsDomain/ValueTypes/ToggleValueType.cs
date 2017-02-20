using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Переключатель")]
    public class ToggleValueType: AbstractValueType
    {
        public ToggleValueType()
        {
            _acceptedValues = new string[] { ValueON, ValueOFF  };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Переключатель";
            }
        }

        public static string ValueON
        {
            get
            {
                return "ON";
            }
        }

        public static string ValueOFF
        {
            get
            {
                return "OFF";
            }
        }
    }
}
