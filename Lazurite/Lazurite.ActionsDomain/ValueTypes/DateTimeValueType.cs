using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;
using System;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Дата и время")]
    [ProtoContract]
    public class DateTimeValueType : ValueTypeBase
    {
        public DateTimeValueType()
        {
            DefaultValueInternal = new DateTime().ToString();
        }

        public override string HumanFriendlyName => "Дата и время";
        public override bool SupportsNumericalComparisons => true;
        public override bool CanBeModified => false;

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(DateTime.TryParse(param, out DateTime @out), param);
    }
}
