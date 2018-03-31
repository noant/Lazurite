using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Object - statistic target (now it can be only scenario)
    /// </summary>
    [DataContract]
    public class StatisticsScenarioInfo
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ValueTypeName { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime Since { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime To { get; set; }
        [DataMember]
        public bool IsEmpty { get; set; }
    }
}
