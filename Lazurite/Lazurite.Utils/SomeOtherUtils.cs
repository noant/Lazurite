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
        private static Dictionary<object, CancellationTokenSource> _cache = new Dictionary<object, CancellationTokenSource>();
        public static void DoManyTimes(object obj, Action action)
        {
            var tokenSource = new CancellationTokenSource();
            if (_cache.ContainsKey(obj))
            {
                _cache[obj].Cancel();
                _cache[obj] = tokenSource;
            }
            else _cache.Add(obj, tokenSource);
            Task.Delay(200, tokenSource.Token)
                .ContinueWith((t) =>
                {
                    _cache.Remove(obj);
                    action();
                });
        }
    }
}
