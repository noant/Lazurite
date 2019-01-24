using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Изображение")]
    [ProtoContract]
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
