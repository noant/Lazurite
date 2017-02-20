using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.ValueTypes
{
    [OnlyGetValue]
    [HumanFriendlyName("Изображение")]
    public class ImageValueType : AbstractValueType
    {
        public ImageValueType()
        {
            _acceptedValues = new string[] { };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Изображение";
            }
        }
    }
}
