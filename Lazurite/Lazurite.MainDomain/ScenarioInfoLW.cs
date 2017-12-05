using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class ScenarioInfoLW
    {
        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public bool IsAvailable { get; set; }

        [DataMember]
        public string CurrentValue { get; set; }
    }
}
