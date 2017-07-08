using Lazurite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lazurite.Utils;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.StandardValueTypeActions;

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
                return new GetStateVTAction() { ValueType = valueType, Value = valueType.AcceptedValues.First() };
            else if (valueType is FloatValueType)
                return new GetFloatVTAction() { ValueType = valueType, Value = valueType.AcceptedValues.First() };
            else if (valueType is ToggleValueType)
                return new GetToggleVTAction() { ValueType = valueType, Value = ToggleValueType.ValueOFF };
            else if (valueType is InfoValueType)
                return new GetInfoVTAction() { ValueType = valueType, Value = string.Empty };
            else if (valueType is DateTimeValueType)
                return new GetDateTimeVTAction() { ValueType = valueType, Value = DateTime.Now.ToString() };
            else return new EmptyAction();
        }
    }
}
