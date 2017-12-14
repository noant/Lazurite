using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [KnownType(typeof(Geolocation))]
    [DataContract]
    public class AddictionalData
    {
        [DataMember]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public AddictionalData()
        {
            //stub
        }

        public AddictionalData(params object[] items)
        {
            foreach (var item in items)
                this.Set(item);
        }

        public void Set(string key, object value)
        {
            if (Data.ContainsKey(key))
                Data[key] = value;
            else Data.Add(key, value);
        }

        public void Set(object value)
        {
            var key = value.GetType().Name;
            Set(key, value);
        }

        public T Resolve<T>()
        {
            var key = typeof(T).Name;
            if (!Data.ContainsKey(key))
                return default(T);
            else return (T)Data[key];
        }

        public object this[string key]
        {
            get
            {
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }

        public bool ContainsKey(string key) => Data.ContainsKey(key);
    }
}
