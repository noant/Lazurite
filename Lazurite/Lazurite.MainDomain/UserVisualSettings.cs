using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class UserVisualSettings
    {
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public AddictionalData AddictionalData { get; set; } = new AddictionalData();

        [DataMember]
        public int PositionX { get; set; }

        [DataMember]
        public int PositionY { get; set; }
        
        public bool SameAs(UserVisualSettings settings)
        {
            return settings.UserId == UserId && settings.ScenarioId.Equals(ScenarioId);
        }
    }
}
