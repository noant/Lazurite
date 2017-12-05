using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Изображение")]
    [DataContract]
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
