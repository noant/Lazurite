using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [ProtoContract]
    [ProtoInclude(1, typeof(ButtonValueType))]
    [ProtoInclude(2, typeof(DateTimeValueType))]
    [ProtoInclude(3, typeof(FloatValueType))]
    [ProtoInclude(4, typeof(GeolocationValueType))]
    [ProtoInclude(5, typeof(ImageValueType))]
    [ProtoInclude(6, typeof(InfoValueType))]
    [ProtoInclude(7, typeof(StateValueType))]
    [ProtoInclude(8, typeof(ToggleValueType))]
    public abstract class ValueTypeBase
    {
        protected string[] _acceptedValues;

        [ProtoMember(1)]
        public string[] AcceptedValues
        {
            get
            {
                if (_acceptedValues == null)
                    _acceptedValues = new string[0];
                return _acceptedValues;
            }
            set => _acceptedValues = value;
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

        public override int GetHashCode()
        {
            var hashCode = -1939736390;
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(_acceptedValues);
            hashCode = hashCode * -1521134295 + CanBeModified.GetHashCode();
            hashCode = hashCode * -1521134295 + SupportsNumericalComparisons.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HumanFriendlyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DefaultValue);
            return hashCode;
        }

        public virtual string DefaultValue => string.Empty;
    }
}
