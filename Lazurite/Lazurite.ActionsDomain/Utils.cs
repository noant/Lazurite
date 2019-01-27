using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lazurite.ActionsDomain
{
    public static class Utils
    {
        private static Dictionary<string, Type> ValueTypes = new Dictionary<string, Type>();

        public static string ExtractHumanFriendlyName(Type type)
        {
            return 
                (type.GetCustomAttributes(typeof(HumanFriendlyNameAttribute), true).FirstOrDefault() as HumanFriendlyNameAttribute)?.Value ?? type.Name;
        }
        
        public static string ExtractHumanFriendlyName(Type type, string memberName)
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
#pragma warning disable IDE0019 // Используйте сопоставление шаблонов
            var attr = type.GetCustomAttributes(typeof(SuitableValueTypesAttribute), true).FirstOrDefault() as SuitableValueTypesAttribute;
#pragma warning restore IDE0019 // Используйте сопоставление шаблонов
            return attr != null ? attr.All || attr.Types.Contains(valueType) : false;
        }

        public static Type GetValueTypeByClassName(string name)
        {
            if (ValueTypes == null)
            {
                var vtBase = typeof(ValueTypeBase);
                var allTypes = typeof(ValueTypeBase).Assembly.GetTypes().Where(x => vtBase.IsAssignableFrom(x)).ToArray();
                ValueTypes = allTypes.ToDictionary((x) => x.Name);
            }
            if (ValueTypes.ContainsKey(name))
                return ValueTypes[name];
            else return ValueTypes[typeof(InfoValueType).Name]; //crutch
        }

        public static string GetValueTypeClassName(Type valueTypeType) => valueTypeType.Name;
    }
}
