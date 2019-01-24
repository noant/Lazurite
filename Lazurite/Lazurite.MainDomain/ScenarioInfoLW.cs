using ProtoBuf;

namespace Lazurite.MainDomain
{
    [ProtoContract]
    public class ScenarioInfoLW
    {
        [ProtoMember(1)]
        public string ScenarioId { get; set; } //guid

        [ProtoMember(2)]
        public bool IsAvailable { get; set; }

        [ProtoMember(3)]
        public string CurrentValue { get; set; }
    }
}
