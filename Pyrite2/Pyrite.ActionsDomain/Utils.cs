using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public static class Utils
    {
        public static string ExtractHumanFriendlyName(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<HumanFriendlyNameAttribute>();
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
    }
}
