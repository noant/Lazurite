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

        public override string HumanFriendlyName => "Изображение";

        public override ValueTypeInterpreteResult Interprete(string param)
        {
            throw new System.NotImplementedException();
        }
    }
}
