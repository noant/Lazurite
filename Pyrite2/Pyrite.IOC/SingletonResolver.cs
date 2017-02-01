using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.IOC
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
            return (T)_object.Single(x => x.GetType().Equals(typeof(T)) || typeof(T).IsAssignableFrom(x.GetType()));
        }
    }
}
