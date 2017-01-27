using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public class FloatValueType: ValueType
    {
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
    }
}
