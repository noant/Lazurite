using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Информация")]
    [ProtoContract]
    public class InfoValueType : ValueTypeBase
    {
        public override string HumanFriendlyName => "Информация";

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(true, param);
    }
}
