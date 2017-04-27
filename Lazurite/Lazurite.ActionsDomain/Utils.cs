using Lazurite.ActionsDomain.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Lazurite.ActionsDomain
{
    public static class Utils
    {
        public static string ExtractHumanFriendlyName(Type type)
        {
            return 
                (type.GetCustomAttributes(typeof(HumanFriendlyNameAttribute), true).FirstOrDefault() as HumanFriendlyNameAttribute)?.Value;
        }

        public static string ExtractHumanFrindlyName(Type type, string memberName)
        {
            var memberInfo = 
                type.GetProperty(memberName) as MemberInfo ??
                type.GetMethod(memberName, new Type[0]) as MemberInfo ??
                type.GetField(memberName) as MemberInfo ??
                type.GetEvent(memberName) as MemberInfo;

            return
                (memberInfo.GetCustomAttributes(typeof(HumanFriendlyNameAttribute), true).FirstOrDefault() as HumanFriendlyNameAttribute)?.Value;
        }

        public static bool IsOnlyExecute(Type type)
        {
            return type.GetCustomAttributes(typeof(OnlyExecuteAttribute), true).Any();
        }

        public static bool IsOnlyGetValue(Type type)
        {
            return type.GetCustomAttributes(typeof(OnlyGetValueAttribute), true).Any();
        }

        public static bool IsCoreVisualInitialization(Type type)
        {
            return type.GetCustomAttributes(typeof(VisualInitializationAttribute), true).Any();
        }

        public static bool IsComparableWithValueType(Type type, Type valueType)
        {
            var attr = type.GetCustomAttributes(typeof(SuitableValueTypesAttribute), true).FirstOrDefault() as SuitableValueTypesAttribute;
            return attr != null ? attr.Types.Contains(valueType) : false;
        }
    }
}
