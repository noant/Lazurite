using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    [OnlyGetValue]
    [HumanFriendlyName("Изображение")]
    public class ImageValueType : ValueType
    {
        public ImageValueType()
        {
            _acceptedValues = new[] { "IMG" };
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
