using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [OnlyGetValue]
    [HumanFriendlyName("Информация")]
    public class InfoValueType : AbstractValueType
    {
        public InfoValueType()
        {
            _acceptedValues = new[] { "INFO" };
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
