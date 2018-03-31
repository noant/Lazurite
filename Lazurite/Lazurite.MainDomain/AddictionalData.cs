using Lazurite.Shared;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [KnownType(typeof(Geolocation))]
    [KnownType(typeof(DeviceInfo))]
    [KnownType(typeof(Messages))]
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
                Set(item);
        }

        public void Set(string key, object value)
        {
            if (Data.ContainsKey(key))
                Data[key] = value;
            else Data.Add(key, value);
        }

        public void Set(object value)
        {
            if (value != null)
            {
                var key = value.GetType().Name;
                Set(key, value);
            }
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
            get => Data[key];
            set => Data[key] = value;
        }

        public bool ContainsKey(string key) => Data.ContainsKey(key);
    }
}
