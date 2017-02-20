using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("ДатаВремя")]
    public class DateTimeValueType: AbstractValueType
    {
        public DateTimeValueType()
        {
            _acceptedValues = new string[] { };
        }

        public override bool SupportsNumericalComparisons
        {
            get
            {
                return true;
            }
        }

        public override bool CanBeModified
        {
            get
            {
                return false;
            }
        }
    }
}
