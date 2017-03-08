using Pyrite.ActionsDomain.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Pyrite.ActionsDomain
{
    public static class Utils
    {
        public static string ExtractHumanFriendlyName(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<HumanFriendlyNameAttribute>();
            if (attr == null)
                return string.Empty;
            return attr.Value;
        }

        public static bool IsOnlyExecute(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<OnlyExecuteAttribute>() != null;
        }

        public static bool IsOnlyGetValue(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<OnlyGetValueAttribute>() != null;
        }

        public static bool IsCoreVisualInitialization(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<VisualInitializationAttribute>() != null;
        }

        public static bool IsComparableWithValueType(Type type, Type valueType)
        {
            return type.GetTypeInfo().GetCustomAttribute<SuitableValueTypesAttribute>().Types.Contains(valueType);
        }

        public static bool IsInhertisValueTypeParams(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<InheritsValueTypeParamsAttribute>() != null;
        }
    }
}
