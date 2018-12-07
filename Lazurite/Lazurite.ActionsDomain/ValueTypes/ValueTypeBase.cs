using System.Linq;
using System.Runtime.Serialization;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [DataContract]
    public abstract class ValueTypeBase
    {
        protected string[] _acceptedValues;

        [DataMember]
        public string[] AcceptedValues {
            get
            {
                if (_acceptedValues == null)
                    _acceptedValues = new string[0];
                return _acceptedValues;
            }
            set {
                _acceptedValues = value;
            }
        }

        public virtual bool CanBeModified => false;

        public virtual bool SupportsNumericalComparisons => false;
        
        public virtual string HumanFriendlyName => "Тип без имени";

        public bool IsCompatibleWith(ValueTypeBase valueType)
        {
            if (valueType == null)
                return false;
            if (this is InfoValueType) return true;
            if (valueType.GetType() != GetType()) return false;
            if (valueType.SupportsNumericalComparisons.Equals(SupportsNumericalComparisons) &&
                Enumerable.SequenceEqual(AcceptedValues, valueType.AcceptedValues))
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTypeBase valueType &&
                GetType() == obj.GetType() &&
                Enumerable.SequenceEqual(AcceptedValues, valueType.AcceptedValues);
        }
        
        public override string ToString() => HumanFriendlyName;

        //интерпретирует входящее значение
        public abstract ValueTypeInterpreteResult Interprete(string param);

        public virtual string DefaultValue => string.Empty;
    }
}
