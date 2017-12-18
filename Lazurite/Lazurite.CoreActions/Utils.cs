using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.ComparisonTypes;
using Lazurite.CoreActions.StandardValueTypeActions;
using Lazurite.Utils;
using System;
using System.Linq;

namespace Lazurite.CoreActions
{
    public static class Utils
    {
        private static IComparisonType[] _comparisonTypes;
        public static IComparisonType[] GetComparisonTypes()
        {
            if (_comparisonTypes == null)
            {
                _comparisonTypes = ReflectionUtils.GetAllOfType(typeof(IComparisonType))
                    .Select(x => (IComparisonType)Activator.CreateInstance(x.AsType())).ToArray();
            }
            return _comparisonTypes;
        }

        public static IAction Default(ValueTypeBase valueType)
        {
            if (valueType is StateValueType)
                return new GetStateVTAction() { ValueType = valueType, Value = valueType.DefaultValue };
            else if (valueType is FloatValueType)
                return new GetFloatVTAction() { ValueType = valueType, Value = valueType.DefaultValue };
            else if (valueType is ToggleValueType)
                return new GetToggleVTAction() { ValueType = valueType, Value = valueType.DefaultValue };
            else if (valueType is InfoValueType)
                return new GetInfoVTAction() { ValueType = valueType, Value = valueType.DefaultValue };
            else if (valueType is DateTimeValueType)
                return new GetDateTimeVTAction() { ValueType = valueType, Value = valueType.DefaultValue };
            else return new EmptyAction();
        }
    }
}
