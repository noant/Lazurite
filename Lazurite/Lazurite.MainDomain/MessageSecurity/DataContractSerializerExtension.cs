using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.MessageSecurity
{
    public static class DataContractSerializerExtension
    {
        public static byte[] Serialize(this DataContractSerializer serializer, object obj)
        {
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }

        public static object Deserialize(this DataContractSerializer serializer, byte[] raw)
        {
            using (var ms = new MemoryStream(raw))
            {
                return serializer.ReadObject(ms);
            }
        }

        public static object Deserialize(this DataContractSerializer serializer, string data)
        {
            return serializer.Deserialize(Encoding.UTF8.GetBytes(data));
        }
    }
}
