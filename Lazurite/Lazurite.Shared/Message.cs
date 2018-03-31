using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Shared
{
    [DataContract]
    public class Message
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime DateTime { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Header { get; set; }
    }
}
