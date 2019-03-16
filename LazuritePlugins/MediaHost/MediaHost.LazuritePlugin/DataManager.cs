using Lazurite.Data;
using Lazurite.IOC;

namespace MediaHost.LazuritePlugin
{
    public class DataManager : MediaHost.Bases.IDataManager
    {
#if !DEBUG
        private static readonly PluginsDataManagerBase InternalData = Singleton.Resolve<PluginsDataManagerBase>();

        public bool TryLoad<T>(string name, out T val)
        {
            try
            {
                val = InternalData.Get<T>(name);
                return true;
            }
            catch
            {
                val = default(T);
                return false;
            }
        }

        public bool Save<T>(string name, T data)
        {
            try
            {
                InternalData.Set<T>(name, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

#endif
#if DEBUG
        private Dictionary<string, object> _temp = new Dictionary<string, object>();

        public bool TryLoad<T>(string name, out T val)
        {
            val = (T)_temp[name];
            return false;
        }

        public bool Save<T>(string name, T data)
        {
            if (!_temp.ContainsKey(name))
                _temp.Add(name, data);
            else
                _temp[name] = data;
            return true;
        }

#endif
    }
}