using System.IO;

namespace Lazurite.Data
{
    public abstract class PluginsDataManagerBase
    {
        public abstract T Get<T>(string key);
        public abstract void Set<T>(string key, T data);
        public abstract void Clear(string key);
        public abstract bool Has(string key);
    }
}