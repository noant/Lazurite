using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.MessageSecurity
{
    [CollectionDataContract]
    public class EncryptedList<T> : List<Encrypted<T>>
    {
        public EncryptedList()
        {
            //
        }

        public EncryptedList(IEnumerable<T> objs, string secretKey)
        {
            this.AddRange(objs.Select(x => new Encrypted<T>(x, secretKey)));
        }

        public List<T> Decrypt(string secretKey)
        {
            return this.Select(x => x.Decrypt(secretKey)).ToList();
        }
    }
}
