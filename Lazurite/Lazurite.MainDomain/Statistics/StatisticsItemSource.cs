using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Source of scenario changes (it can be user, other scenario or network)
    /// </summary>
    [DataContract]
    public class StatisticsItemSource
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SourceType { get; set; }
    }
}
