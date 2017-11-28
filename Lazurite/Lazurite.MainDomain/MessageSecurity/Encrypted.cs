using Lazurite.IOC;
using Lazurite.Logging;
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
        private static ILogger Log = Singleton.Resolve<ILogger>();
        private static Dictionary<string, SecureEncoding> Cached = new Dictionary<string, SecureEncoding>();
        private static object Locker_GetSecureEncoding = new object();
        private static SecureEncoding GetSecureEncoding(string key)
        {
            lock (Locker_GetSecureEncoding)
            {
                if (!Cached.ContainsKey(key))
                    Cached.Add(key, new SecureEncoding(key));
                return Cached[key];
            }
        }

        [DataMember]
        public string Data { get; set; }

        public Encrypted()
        {
            //do nothing
        }

        public Encrypted(T obj, string secretKey): this()
        {
            Log.Debug("Encrypted object creating...");
            var serializer = SerializersFactory.GetSerializer<T>();
            Data = GetSecureEncoding(secretKey).Encrypt(serializer.Serialize(obj));
            ServerTime = DateTime.Now;
            Log.Debug("Encrypted object created");
        }

        [DataMember]
        public DateTime ServerTime { get; set; }

        public T Decrypt(string secretKey)
        {
            Log.Debug("Decryption begin...");
            try
            {
                var secureEncoding = GetSecureEncoding(secretKey);
                var serializer = SerializersFactory.GetSerializer<T>();
                var decryptedRaw = secureEncoding.Decrypt(Data);
                return (T)serializer.Deserialize(decryptedRaw.TrimEnd('\0'));
            }
            catch (Exception e)
            {
                Log.WarnFormat(e, "Ошибка при расшифровке строки");
                throw new DecryptException(e);
            }
            finally
            {
                Log.Debug("Decryption end...");
            }
        }
    }

    public class DecryptException : Exception
    {
        public DecryptException(Exception inner) : base("Decryption error", inner)
        {
            //do nothing
        }
    }
}
