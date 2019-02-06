using ProtoBuf;
using System.Linq;

namespace Lazurite.MainDomain.Statistics
{
    [ProtoContract]
    public class ScenarioStatisticsRegistration
    {
        public ScenarioStatisticsRegistration(string[] registeredIds) =>
            RegisteredIds = registeredIds;

        public ScenarioStatisticsRegistration() { } //empty

        [ProtoMember(1, OverwriteList = true)]
        public string[] RegisteredIds { get; set; }

        public bool IsRegistered(string scenarioId) =>
            RegisteredIds.Contains(scenarioId);

        public void Union(ScenarioStatisticsRegistration info)
        {
            if (RegisteredIds == null)
                RegisteredIds = info.RegisteredIds.ToArray();
            else if (info.RegisteredIds != null)
                RegisteredIds = RegisteredIds.Union(info.RegisteredIds).Distinct().ToArray();
        }
    }
}
