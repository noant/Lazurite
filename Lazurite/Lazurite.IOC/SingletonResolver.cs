using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lazurite.IOC
{
    public static class Singleton
    {
        private static List<object> _object = new List<object>();

        public static void Add(object obj)
        {
            _object.Add(obj);
        }

        public static T Resolve<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            return (T)_object.Single(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
        }

        public static bool Any<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            return _object.Any(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
        }        
    }
}
