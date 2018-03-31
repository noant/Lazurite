using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Lazurite.MainDomain.MessageSecurity
{
    [DataContract]
    public class EncryptedList<T>
    {
        [DataMember]
        public List<Encrypted<T>> SourceList { get; set; } = new List<Encrypted<T>>();

        [DataMember]
        public DateTime ServerTime { get; set; }

        public EncryptedList()
        {
            ServerTime = DateTime.Now.ToUniversalTime();
        }
        
        public EncryptedList(IEnumerable<T> objs, string secretKey) : this()
        {
            SourceList.AddRange(objs.Select(x => new Encrypted<T>(x, secretKey, true)));
        }

        public List<T> Decrypt(string secretKey)
        {
            return SourceList.Select(x => x.Decrypt(secretKey)).ToList();
        }
    }
}
