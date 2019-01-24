using ProtoBuf;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Source of scenario changes (it can be user, other scenario or network)
    /// </summary>
    [ProtoContract]
    public class StatisticsItemSource
    {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string SourceType { get; set; }
    }
}
