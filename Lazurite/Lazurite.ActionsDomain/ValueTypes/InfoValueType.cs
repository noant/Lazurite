using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [OnlyGetValue]
    [HumanFriendlyName("Информация")]
    public class InfoValueType : ValueTypeBase
    {
        public InfoValueType()
        {
            _acceptedValues = new string[] { };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Информация";
            }
        }
    }
}
