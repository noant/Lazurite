using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lazurite.MainDomain
{
    public static class GlobalSettings
    {
        private static Dictionary<string, ObjectCrutch> Data;
        private static DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();
        private static readonly object Locker = new object();

        public static T Get<T>(T @default = default(T), string key = "", [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            try
            {
                sourceFilePath = new Uri(sourceFilePath).Segments.Last();
                key = Path.Combine(sourceFilePath, memberName, key);
                lock (Locker)
                {
                    if (Data == null)
                    {
                        if (DataManager.Has(nameof(GlobalSettings)))
                            Data = DataManager.Get<Dictionary<string, ObjectCrutch>>(nameof(GlobalSettings));
                        else
                            Data = new Dictionary<string, ObjectCrutch>();
                    }
                    if (!Data.ContainsKey(key))
                    {
                        Data.Add(key, ObjectCrutch.From(@default));
                        DataManager.Set(nameof(GlobalSettings), Data);
                        return @default;
                    }
                    else
                    {
                        return (T)Data[key].Object;
                    }
                }
            }
            catch
            {
                return @default;
            }
        }

        public class ObjectCrutch
        {
            public static ObjectCrutch From(object obj)
            {
                return new ObjectCrutch()
                {
                    Object = obj
                };
            }

            public object Object { get; set; }
        }
    }
}
