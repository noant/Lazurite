using System;
using System.Linq;
using System.Reflection;

namespace Lazurite.Utils
{
    public static class ReflectionUtils
    {
        public static TypeInfo[] GetAllOfType(Type type, Assembly assembly=null)
        {
            var typeInfo = type.GetTypeInfo();
            if (assembly == null)
                assembly = typeInfo.Assembly;
            return assembly.DefinedTypes
                    .Where(x => typeInfo.IsAssignableFrom(x) && !x.IsInterface).ToArray();
        }

        public static TypeInfo[] GetAllOfType(Type type, Type[] types)
        {
            var typeInfo = type.GetTypeInfo();
            return types
                .Select(x=>x.GetTypeInfo())
                .Where(x => typeInfo.IsAssignableFrom(x) && !x.IsInterface).ToArray();
        }
    }
}
