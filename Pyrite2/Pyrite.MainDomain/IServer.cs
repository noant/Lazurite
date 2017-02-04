using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public interface IServer
    {
        bool IsScenarioValueChanged(string scenarioId, DateTime since, string lastKnownValue);
        ScenarioPackage[] GetScenarioPackages();
        ScenarioPackage[] GetChangedScenarios(DateTime since);
        string GetScenarioValue(string scenarioId);
        void SetScenarioValue(string scenarioId, string value);
    }
}
