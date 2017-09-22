using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
