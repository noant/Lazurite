using Lazurite.ActionsDomain.Attributes;
using System.Linq;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Статус")]
    [DataContract]
    public class StateValueType : ValueTypeBase
    {
        public StateValueType()
        {
            AcceptedValues = new string[] { "Статус 1", "Статус 2", "Статус 3" };
        }

        public override bool CanBeModified => true;

        public override string HumanFriendlyName => "Статус";

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(AcceptedValues.Contains(param), param);

        public override string DefaultValue => AcceptedValues.FirstOrDefault();
    }
}
