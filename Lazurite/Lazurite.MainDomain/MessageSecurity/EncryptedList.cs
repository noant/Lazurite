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
        public Encrypted<List<T>> SourceList { get; set; } = new Encrypted<List<T>>();

        [DataMember]
        public SafeDateTime ServerTime { get; set; }

        public EncryptedList()
        {
            ServerTime = SafeDateTime.FromDateTime(DateTime.Now);
        }
        
        public EncryptedList(IEnumerable<T> objs, string secretKey) : this()
        {
            SourceList = new Encrypted<List<T>>(objs.ToList(), secretKey);
        }

        public List<T> Decrypt(string secretKey)
        {
            return SourceList.Decrypt(secretKey);
        }
    }
}
