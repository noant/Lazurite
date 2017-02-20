using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Статус")]
    public class StateValueType : AbstractValueType
    {
        public override bool CanBeModified
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
                return "Статус";
            }
        }
    }
}
