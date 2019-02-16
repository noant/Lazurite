using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;
using System.Linq;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Статус")]
    [ProtoContract]
    public class StateValueType : ValueTypeBase
    {
        public StateValueType()
        {
            AcceptedValues = new string[] { "Статус 1", "Статус 2", "Статус 3" };
            DefaultValueInternal = AcceptedValues[0];
        }

        public override bool CanBeModified => true;

        public override string HumanFriendlyName => "Статус";

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(AcceptedValues.Contains(param), param);

        public override string DefaultValue {
            get => string.IsNullOrEmpty(DefaultValueInternal) || !AcceptedValues.Contains(DefaultValueInternal) ? 
                AcceptedValues.FirstOrDefault() : 
                DefaultValueInternal;
        }
    }
}
