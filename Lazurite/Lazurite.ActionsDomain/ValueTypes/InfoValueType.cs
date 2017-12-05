using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Информация")]
    [DataContract]
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
