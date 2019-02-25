using System.Collections.Generic;

namespace Lazurite.Data
{
    /// <summary>
    /// Read/write and store parameters
    /// </summary>
    public class StoredPropertiesManager
    {
        private static readonly DataManagerBase DataManager = IOC.Singleton.Resolve<DataManagerBase>();

        private readonly Dictionary<string, object> _store = new Dictionary<string, object>();

        public virtual T Get<T>(string id, T @default = default(T))
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new System.ArgumentException("Id cannot be empty or white space", nameof(id));
            }

            if (_store.ContainsKey(id))
            {
                return (T)_store[id];
            }
            else
            {
                if (DataManager.Has(id))
                {
                    var data = DataManager.Get<T>(id);
                    _store.Add(id, data);
                    return data;
                }
                else
                {
                    return @default;
                }
            }
        }

        public virtual void Set<T>(string id, T obj)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new System.ArgumentException("Id cannot be empty or white space", nameof(id));
            }

            if (_store.ContainsKey(id))
            {
                _store[id] = obj;
            }
            else
            {
                _store.Add(id, obj);
            }

            DataManager.Set(id, obj);
        }

        public virtual void Reset(string id)
        {
            if (_store.ContainsKey(id))
            {
                _store.Remove(id);
                DataManager.Clear(id);
            }
        }
    }
}