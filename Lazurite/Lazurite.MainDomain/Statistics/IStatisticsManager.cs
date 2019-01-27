using ProtoBuf;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    public interface IStatisticsManager
    {
        Task<StatisticsScenarioInfo> GetStatisticsInfoForScenario(ScenarioBase scenario, ScenarioActionSource source);
        Task<StatisticsItem[]> GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to, ScenarioActionSource source);
        void Register(ScenarioBase scenario);
        void UnRegister(ScenarioBase scenario);
        Task<ScenarioStatisticsRegistration> GetRegistrationInfo(ScenarioBase[] scenarios);
        void ReRegister(ScenarioBase scenario);
    }

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
                RegisteredIds = new string[0];
            if (info.RegisteredIds != null)
                RegisteredIds = RegisteredIds.Union(info.RegisteredIds).Distinct().ToArray();
        }
    }
}