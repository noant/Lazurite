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
            base.AcceptedValues = new string[] { 0.0.ToString(), 100.0.ToString() };
        }
        
        public override bool CanBeModified
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsNumericalComparisons
        {
            get
            {
                return true;
            }
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Число";
            }
        }
    }
}
