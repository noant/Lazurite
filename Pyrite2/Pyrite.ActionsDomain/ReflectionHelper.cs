using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public static class ReflectionHelper
    {
        public static string ExtractHumanFriendlyName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(HumanFriendlyNameAttribute), true).FirstOrDefault();
            return ((HumanFriendlyNameAttribute)attr).Value;
        }

        public static bool IsOnlyExecute(Type type)
        {
            return type.GetCustomAttributes(typeof(OnlyExecuteAttribute), true).Any();
        }

        public static bool IsOnlyGetValue(Type type)
        {
            return type.GetCustomAttributes(typeof(OnlyGetValueAttribute), true).Any();
        }
    }
}
