using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.CoreActions
{
    public interface ICoreAction
    {
        string TargetScenarioId { get; }
        ScenarioBase GetTargetScenario();
        void SetTargetScenario(ScenarioBase scenario);
    }
}