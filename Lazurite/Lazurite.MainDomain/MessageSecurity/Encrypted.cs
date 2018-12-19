using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

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
        public string Salt { get; set; }

        [DataMember]
        public string Data { get; set; }

        public Encrypted()
        {
            //do nothing
        }

        public Encrypted(T obj, string secretKey, bool nullServerTime = false): this()
        {
            Log.Debug("Encrypted object creating...");
            var serializer = SerializersFactory.GetSerializer<T>();
            Salt = SecureEncoding.CreateSalt();
            var iv = SecureEncoding.CreateIV(Salt, secretKey);
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;
                Data = GetSecureEncoding(secretKey).Encrypt(ms.ToArray(), iv);
            }
            if (!nullServerTime)
                ServerTime = SafeDateTime.FromDateTime(DateTime.Now);
            Log.Debug("Encrypted object created");
        }

        [DataMember]
        public SafeDateTime ServerTime { get; set; }

        public T Decrypt(string secretKey)
        {
            Log.Debug("Decryption begin...");
            try
            {
                var iv = SecureEncoding.CreateIV(Salt, secretKey);
                var secureEncoding = GetSecureEncoding(secretKey);
                var serializer = SerializersFactory.GetSerializer<T>();
                var decryptedRaw = secureEncoding.DecryptBytes(Data, iv);
                using (var ms = new MemoryStream(decryptedRaw))
                    return (T)serializer.ReadObject(ms);
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
