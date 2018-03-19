using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Info of one statistics change
    /// </summary>
    [DataContract]
    public class StatisticsItem
    {
        [DataMember]
        public DateTime DateTime { get; set; }
        [DataMember]
        public StatisticsItemSource Source { get; set; }
        [DataMember]
        public StatisticsScenarioInfo Target { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string ValueTypeName { get; set; }
    }
}
