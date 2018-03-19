using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    public interface IStatisticsManager
    {
        StatisticsScenarioInfo GetSelectionItemForScenario(string scenarioId);
        StatisticsItem[] GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to);
        void Register(ScenarioBase scenario);
        void UnRegister(ScenarioBase scenario);
        void IsRegistered(ScenarioBase scenario);
    }
}
