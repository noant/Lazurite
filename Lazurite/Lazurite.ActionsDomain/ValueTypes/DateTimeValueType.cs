using Lazurite.ActionsDomain.Attributes;
using System;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Дата и время")]
    [DataContract]
    public class DateTimeValueType: ValueTypeBase
    {
        private static readonly DateTime DefaultDateTime = new DateTime();

        public DateTimeValueType()
        {
            _acceptedValues = new string[] { };
        }

        public override string HumanFriendlyName => "Дата и время";
        public override bool SupportsNumericalComparisons => true;
        public override bool CanBeModified => false;

        public override ValueTypeInterpreteResult Interprete(string param) => new ValueTypeInterpreteResult(DateTime.TryParse(param, out DateTime @out), param);

        public override string DefaultValue => DefaultDateTime.ToString();
    }
}
