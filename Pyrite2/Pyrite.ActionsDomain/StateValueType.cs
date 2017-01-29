using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    [HumanFriendlyName("Статус")]
    public class StateValueType : ValueType
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
