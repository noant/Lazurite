using System.Collections.Generic;

namespace Lazurite.MainDomain
{
    public class AddictionalData: Dictionary<string, object>
    {
        public void Set(string key, object value)
        {
            if (this.ContainsKey(key))
                this[key] = value;
            else this.Add(key, value);
        }

        public void Set(object value)
        {
            var key = value.GetType().Name;
            Set(key, value);
        }

        public T Resolve<T>()
        {
            var key = typeof(T).Name;
            if (!this.ContainsKey(key))
                return default(T);
            else return (T)this[key];
        }
    }
}
