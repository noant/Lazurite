using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Число")]
    [DataContract]
    public class FloatValueType: ValueTypeBase
    {
        public FloatValueType()
        {
            AcceptedValues = new string[] { 0.0.ToString(), 100.0.ToString() };
        }

        public override bool CanBeModified => true;
        public override bool SupportsNumericalComparisons => true;
        public override string HumanFriendlyName => "Число";
        public double Max => double.Parse(AcceptedValues[1]);
        public double Min => double.Parse(AcceptedValues[0]);

        [DataMember]
        public string Unit { get; set; }

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(double.TryParse(param, out double @out) && @out <= Max && @out >= Min, param);

        public override string DefaultValue => Min.ToString();
    }
}
