using ProtoBuf;
using System;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Info of one statistics change
    /// </summary>
    [ProtoContract]
    public class StatisticsItem
    {
        [ProtoMember(1)]
        public DateTime DateTime { get; set; }
        [ProtoMember(2)]
        public StatisticsItemSource Source { get; set; }
        [ProtoMember(3)]
        public StatisticsScenarioInfo Target { get; set; }
        [ProtoMember(4)]
        public string Value { get; set; }
    }
}
