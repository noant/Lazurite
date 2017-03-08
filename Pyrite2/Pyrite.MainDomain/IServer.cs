using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public interface IServer
    {
        bool IsScenarioValueChanged(string scenarioId, string lastKnownValue);
        ScenarioInfo[] GetScenariosInfo();
        ScenarioInfo GetScenarioInfo(string scenarioId);
        string CalculateScenarioValue(string scenarioId);
        string GetScenarioValue(string scenarioId);
        void ExecuteScenario(string scenarioId, string value);
        void ExecuteScenarioAsync(string scenarioId, string value);
        void ExecuteScenarioAsyncParallel(string scenarioId, string value);
        ScenarioInfoLW[] GetChangedScenarios(DateTime since);
    }
}
