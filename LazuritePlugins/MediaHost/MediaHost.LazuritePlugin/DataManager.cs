using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.LazuritePlugin
{
    public class DataManager : MediaHost.Bases.IDataManager
    {
#if !DEBUG
        private static readonly PluginsDataManagerBase InternalData = Singleton.Resolve<PluginsDataManagerBase>();

        public bool Exists(string name) => InternalData.Has(name);

        public T Load<T>(string name) => InternalData.Get<T>(name);

        public void Save<T>(string name, T data) => InternalData.Set<T>(name, data);
#endif
#if DEBUG
        private Dictionary<string, object> _temp = new Dictionary<string, object>();

        public bool Exists(string name) => _temp.ContainsKey(name);

        public T Load<T>(string name) => (T)_temp[name];

        public void Save<T>(string name, T data) {
            if (!Exists(name))
                _temp.Add(name, data);
            else
                _temp[name] = data;
        }
#endif
    }
}
