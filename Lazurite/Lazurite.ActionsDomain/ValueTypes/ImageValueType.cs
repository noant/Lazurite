using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [OnlyGetValue]
    [HumanFriendlyName("Изображение")]
    public class ImageValueType : ValueTypeBase
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
