using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    [DataContract]
    public class UserVisualSettings
    {
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public byte[] Color { get; set; } // 3 bytes

        [DataMember]
        public int PositionX { get; set; }

        [DataMember]
        public int PositionY { get; set; }
        
        public bool SameAs(UserVisualSettings settings)
        {
            return settings.UserId.Equals(UserId) && settings.ScenarioId.Equals(ScenarioId);
        }
    }
}
