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
        public StateValueType()
        {
            AcceptedValues = new string[] { "Статус 1", "Статус 2", "Статус 3" };
        }

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
