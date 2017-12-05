using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lazurite.IOC
{
    public static class Singleton
    {
        private static List<object> _objects = new List<object>();

        public static void Add(object obj)
        {
            _objects.Add(obj);
        }

        public static T Resolve<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            return (T)_objects.Single(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
        }

        public static bool Any<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            return _objects.Any(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
        }

        public static void Clear<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            _objects.RemoveAll(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
        }
    }
}
