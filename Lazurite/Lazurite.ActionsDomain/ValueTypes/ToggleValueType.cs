using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Переключатель")]
    public class ToggleValueType: ValueTypeBase
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
                return true.ToString();
            }
        }

        public static string ValueOFF
        {
            get
            {
                return false.ToString();
            }
        }
    }
}
