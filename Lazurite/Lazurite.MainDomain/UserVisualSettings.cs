using ProtoBuf;

namespace Lazurite.MainDomain
{
    [ProtoContract]
    public class UserVisualSettings
    {
        [ProtoMember(1)]
        public string UserId { get; set; }

        [ProtoMember(2)]
        public string ScenarioId { get; set; } //guid

        [ProtoMember(3)]
        public AddictionalData AddictionalData { get; set; } = new AddictionalData();

        [ProtoMember(4)]
        public int VisualIndex { get; set; } = int.MaxValue;
        
        public bool SameAs(UserVisualSettings settings)
        {
            return settings.UserId == UserId && settings.ScenarioId.Equals(ScenarioId);
        }
    }
}
