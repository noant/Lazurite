using Pyrite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions
{
    public static class Utils
    {
        private static Assembly[] GetEntireAssemblies()
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

        private static IComparisonType[] _comparisonTypes;
        public static IComparisonType[] GetComparisonTypes()
        {
            if (_comparisonTypes == null)
            {
                _comparisonTypes = GetAllOfType(typeof(IComparisonType))
                    .Select(x => (IComparisonType)Activator.CreateInstance(x.AsType())).ToArray();
            }
            return _comparisonTypes;
        }
    }
}
