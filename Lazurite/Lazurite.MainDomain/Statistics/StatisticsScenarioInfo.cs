using ProtoBuf;
using System;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Object - statistic target (now it can be only scenario)
    /// </summary>
    [ProtoContract]
    public class StatisticsScenarioInfo
    {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string ValueTypeName { get; set; }
        [ProtoMember(4)]
        public DateTime Since { get; set; }
        [ProtoMember(5)]
        public DateTime To { get; set; }
        [ProtoMember(6)]
        public bool IsEmpty { get; set; }
    }
}
