using System;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    public interface IStatisticsManager
    {
        Task<StatisticsScenarioInfo> GetStatisticsInfoForScenario(ScenarioBase scenario, ScenarioActionSource source);
        Task<ScenarioStatistic> GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to, ScenarioActionSource source);
        void Register(ScenarioBase scenario);
        void UnRegister(ScenarioBase scenario);
        Task<ScenarioStatisticsRegistration> GetRegistrationInfo(ScenarioBase[] scenarios);
        void ReRegister(ScenarioBase scenario);
    }
}