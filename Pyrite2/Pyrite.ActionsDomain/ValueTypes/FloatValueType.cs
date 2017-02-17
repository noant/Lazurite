using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Число")]
    public class FloatValueType: AbstractValueType
    {
        public FloatValueType()
        {
            base.AcceptedValues = new string[] { int.MinValue.ToString(), int.MaxValue.ToString() };
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
