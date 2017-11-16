using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public static class GlobalSettings
    {
        private static Dictionary<string, object> Data;
        private static SaviorBase Savior = Singleton.Resolve<SaviorBase>();
        private static object Locker = new object();

        public static T Get<T>(string key, T @default = default(T))
        {
            lock (Locker)
            {
                if (Data == null)
                {
                    if (Savior.Has(nameof(GlobalSettings)))
                        Data = Savior.Get<Dictionary<string, object>>(nameof(GlobalSettings));
                    else
                        Data = new Dictionary<string, object>();
                }
                if (!Data.ContainsKey(key))
                {
                    Data.Add(key, @default);
                    Savior.Set(nameof(GlobalSettings), Data);
                    return @default;
                }
                else
                    return (T)Data[key];
            }
        }
    }
}
