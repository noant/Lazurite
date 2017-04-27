using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.MessageSecurity
{
    [DataContract]
    public class Encrypted<T>
    {
        private static Dictionary<string, SecureEncoding> Cached = new Dictionary<string, SecureEncoding>();
        private static SecureEncoding GetSecureEncoding(string key)
        {
            if (!Cached.ContainsKey(key))
            {
                Cached.Add(key, new SecureEncoding(key));
            }
            return Cached[key];
        }

        [DataMember]
        public string Data { get; set; }

        public Encrypted()
        {
            //do nothing
        }

        public Encrypted(T obj, string secretKey)
        {
            var serializer = SerializersFactory.GetSerializer<T>();
            Data = GetSecureEncoding(secretKey).Encrypt(serializer.Serialize(obj));
        }

        public T Decrypt(string secretKey)
        {
            var secureEncoding = GetSecureEncoding(secretKey);
            var serializer = SerializersFactory.GetSerializer<T>();
            var decryptedRaw = secureEncoding.Decrypt(Data);
            return (T)serializer.Deserialize(decryptedRaw.TrimEnd('\0'));
        }
    }
}
