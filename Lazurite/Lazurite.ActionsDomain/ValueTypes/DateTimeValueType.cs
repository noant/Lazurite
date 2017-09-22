using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Дата и время")]
    [DataContract]
    public class DateTimeValueType: ValueTypeBase
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
