using OpenZWrapper;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWavePlugin
{
    public static class ZWaveTypeComparability
    {
        public static Type GetTypeBy(OpenZWrapper.ValueType zWaveValueType) {
            switch (zWaveValueType)
            {
                case OpenZWrapper.ValueType.Bool:
                    return typeof(Pyrite.ActionsDomain.ValueTypes.ToggleValueType);
                case OpenZWrapper.ValueType.Button:
                    return typeof(Pyrite.ActionsDomain.ValueTypes.ButtonValueType);
                case OpenZWrapper.ValueType.Byte:
                case OpenZWrapper.ValueType.Decimal:
                case OpenZWrapper.ValueType.Short:
                case OpenZWrapper.ValueType.Int:
                    return typeof(Pyrite.ActionsDomain.ValueTypes.FloatValueType);
                case OpenZWrapper.ValueType.String:
                    return typeof(Pyrite.ActionsDomain.ValueTypes.InfoValueType);
                case OpenZWrapper.ValueType.List:
                    return typeof(Pyrite.ActionsDomain.ValueTypes.StateValueType);
            }
            return typeof(ValueTypeBase);
        }

        public static bool IsTypesComparable(NodeValue nodeValue, ValueTypeBase valueType, bool inheritsStates)
        {
            if (valueType == null)
                return true;
            if (valueType is StateValueType && nodeValue.ValueType == OpenZWrapper.ValueType.List &&
                (nodeValue.PossibleValues.OrderBy(x => x).SequenceEqual(((StateValueType)valueType).AcceptedValues.OrderBy(x => x)) || !inheritsStates))
                return true;
            if (GetTypeBy(nodeValue.ValueType).Equals(valueType.GetType()) && nodeValue.ValueType != OpenZWrapper.ValueType.List)
                return true;
            return false;
        }

        public static ValueTypeBase CreateValueTypeFromNodeValue(NodeValue nodeValue)
        {
            var type = GetTypeBy(nodeValue.ValueType);
            var typeInstance = Activator.CreateInstance(type);
            if (nodeValue.ValueType == OpenZWrapper.ValueType.List)
            {
                ((StateValueType)typeInstance).AcceptedValues = nodeValue.PossibleValues;
            }
            return (ValueTypeBase)typeInstance;
        } 
    }
}
