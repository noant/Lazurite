using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
#if DEBUG
            try
            {
                return (T)_object.Single(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
            }
            catch
            {
                return default(T);
            }
#endif
#if !DEBUG
            return (T)_object.Single(x => x.GetType().Equals(typeof(T)) || typeInfo.IsAssignableFrom(x.GetType().GetTypeInfo()));
#endif
        }
    }
}
