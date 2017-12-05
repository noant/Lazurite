using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [OnlyExecute]
    [HumanFriendlyName("Кнопка")]
    [DataContract]
    public class ButtonValueType : ValueTypeBase
    {
        public ButtonValueType() {
            _acceptedValues = new string[] { };
        }

        public override string HumanFriendlyName
        {
            get
            {
                return "Кнопка";
            }
        }
    }
}
