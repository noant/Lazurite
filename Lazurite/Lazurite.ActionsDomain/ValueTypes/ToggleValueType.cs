using Lazurite.ActionsDomain.Attributes;
using System.Linq;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Переключатель")]
    [DataContract]
    public class ToggleValueType: ValueTypeBase
    {
        public static readonly string ValueON = true.ToString();
        public static readonly string ValueOFF = false.ToString();

        public ToggleValueType()
        {
            _acceptedValues = new string[] { ValueON, ValueOFF  };
        }

        public override string HumanFriendlyName => "Переключатель";

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(AcceptedValues.Contains(param), param);

        public override string DefaultValue => ValueOFF;
    }
}
