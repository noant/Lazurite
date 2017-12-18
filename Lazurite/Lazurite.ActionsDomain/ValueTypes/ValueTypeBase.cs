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
            get {
                return _acceptedValues;
            }
            set {
                _acceptedValues = value;
            }
        }
        
        public virtual bool CanBeModified
        {
            get
            {
                return false;
            }
        }
        
        public virtual bool SupportsNumericalComparisons
        {
            get
            {
                return false;
            }
        }
        
        public virtual string HumanFriendlyName
        {
            get
            {
                return "Тип без имени";
            }
        }

        public bool IsCompatibleWith(ValueTypeBase valueType)
        {
            if (this is InfoValueType) return true;
            if (valueType.GetType() != this.GetType()) return false;
            if (valueType.SupportsNumericalComparisons.Equals(this.SupportsNumericalComparisons) &&
                Enumerable.SequenceEqual(this.AcceptedValues, valueType.AcceptedValues))
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTypeBase &&
                this.GetType() == obj.GetType() &&
                Enumerable.SequenceEqual(this.AcceptedValues, ((ValueTypeBase)obj).AcceptedValues);
        }
        
        public override string ToString()
        {
            return this.HumanFriendlyName;
        }

        //интерпертирует входящее значение
        public abstract ValueTypeInterpreteResult Interprete(string param);

        public virtual string DefaultValue => string.Empty;
    }
}
