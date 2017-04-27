using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Статус")]
    public class StateValueType : ValueTypeBase
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
