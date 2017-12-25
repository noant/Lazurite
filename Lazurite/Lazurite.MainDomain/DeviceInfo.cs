using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class DeviceInfo
    {
        [DataMember]
        public string Name { get; set; }
    }
}
