using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pyrite.Utils
{
    public static class ReflectionUtils
    {
        public static Assembly[] GetEntireAssemblies()
        {
            var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getassemblies = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
            return getassemblies.Invoke(currentdomain, new object[] { }) as Assembly[];
        }

        public static TypeInfo[] GetAllOfType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var asseblies = GetEntireAssemblies();
            return asseblies.SelectMany(x => x.DefinedTypes)
                    .Where(x => typeInfo.IsAssignableFrom(x) && !x.IsInterface).ToArray();
        }

        public static Assembly GetExecutingAssembly()
        {
            var assm = typeof(string).GetTypeInfo().Assembly;
            var assmType = assm.GetType("System.Reflection.Assembly");
            var methodGetExecAssm = assmType.GetRuntimeMethod("GetExecutingAssembly", new Type[] { });
            return methodGetExecAssm.Invoke(null, new object[] { }) as Assembly;
        }
    }
}
