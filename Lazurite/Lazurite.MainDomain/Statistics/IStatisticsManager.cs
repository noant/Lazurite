using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    public interface IStatisticsManager
    {
        StatisticsScenarioInfo GetStatisticsInfoForScenario(ScenarioBase scenario, ScenarioActionSource source);
        StatisticsItem[] GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to, ScenarioActionSource source);
        void Register(ScenarioBase scenario);
        void UnRegister(ScenarioBase scenario);
        ScenarioStatisticsRegistration GetRegistrationInfo(ScenarioBase[] scenarios);
        void ReRegister(ScenarioBase scenario);
    }

    [DataContract]
    public class ScenarioStatisticsRegistration
    {
        public ScenarioStatisticsRegistration(string[] registeredIds) =>
            RegisteredIds = registeredIds;

        public ScenarioStatisticsRegistration() { } //empty

        [DataMember]
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