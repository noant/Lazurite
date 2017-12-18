using Lazurite.ActionsDomain.Attributes;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [OnlyExecute]
    [HumanFriendlyName("Кнопка")]
    [DataContract]
    public class ButtonValueType : ValueTypeBase
    {
        private static readonly ValueTypeInterpreteResult InterpreteResult = new ValueTypeInterpreteResult(true, string.Empty);

        public ButtonValueType() {
            _acceptedValues = new string[] { };
        }

        public override string HumanFriendlyName => "Кнопка";

        public override ValueTypeInterpreteResult Interprete(string param)
        {
            if (string.IsNullOrEmpty(param))
                return InterpreteResult;
            else return new ValueTypeInterpreteResult(false, param);
        }
    }
}
