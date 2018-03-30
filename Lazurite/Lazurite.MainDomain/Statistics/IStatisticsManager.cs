using System;
using System.Collections.Generic;
using System.Linq;
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
        bool IsRegistered(ScenarioBase scenario);
        void ReRegister(ScenarioBase scenario);
    }
}