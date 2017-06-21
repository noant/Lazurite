using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.Utils
{
    public static class SomeOtherUtils
    {
        private static Dictionary<object, Action> _cache = new Dictionary<object, Action>();
        public static void DoManyTimes(object obj, Action action)
        {
            if (_cache.ContainsKey(obj))
                _cache[obj] = action;
            else
            {
                _cache.Add(obj, action);
                Task.Delay(500)
                    .ContinueWith((t) =>
                    {
                        action = _cache[obj];
                        action();
                        _cache.Remove(obj);
                    });
            }
        }
    }
}
