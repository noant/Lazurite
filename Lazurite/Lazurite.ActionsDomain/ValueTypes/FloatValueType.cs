using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Число")]
    [ProtoContract]
    public class FloatValueType: ValueTypeBase
    {
        public FloatValueType()
        {
            AcceptedValues = new string[] { 0.0.ToString(), 100.0.ToString() };
        }

        private double _default = 0;

        public override bool CanBeModified => true;
        public override bool SupportsNumericalComparisons => true;
        public override string HumanFriendlyName => "Число";
        public double Max => double.Parse(AcceptedValues[1]);
        public double Min => double.Parse(AcceptedValues[0]);

        [ProtoMember(10)]
        public string Unit { get; set; }

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(double.TryParse(param, out double @out) && @out <= Max && @out >= Min, param);

        public override string DefaultValue
        {
            get {
                if (_default > Max || _default < Min)
                    _default = Min;
                return _default.ToString();
            }
            set => double.TryParse(value, out _default);
        }
    }
}
