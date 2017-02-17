using System;

namespace Pyrite.ActionsDomain.ValueTypes
{
    public abstract class AbstractValueType
    {
        protected string[] _acceptedValues;
        public string[] AcceptedValues {
            get {
                return _acceptedValues;
            }
            set {
                if (CanBeModified)
                    _acceptedValues = value;
                else throw new MemberAccessException();
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

        public bool IsCompatibleWith(AbstractValueType valueType)
        {
            if (valueType.SupportsNumericalComparisons.Equals(this.SupportsNumericalComparisons) && valueType.AcceptedValues.Length.Equals(this.AcceptedValues.Length))
            {
                for (int i = 0; i < valueType.AcceptedValues.Length; i++)
                    if (!this.AcceptedValues[i].Equals(valueType.AcceptedValues[i]))
                        return false;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return this.HumanFriendlyName;
        }
    }
}
