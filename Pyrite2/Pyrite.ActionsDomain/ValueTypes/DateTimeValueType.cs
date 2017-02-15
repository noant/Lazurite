using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Дата и время")]
    public class DateTimeValueType: ValueType
    {
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
