using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Переключатель")]
    [DataContract]
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
