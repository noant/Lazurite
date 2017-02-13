using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.CoreActions
{
    public interface ICoreAction
    {
        string TargetScenarioId { get; }
        ScenarioBase GetTargetScenario();
        void SetTargetScenario(ScenarioBase scenario);
    }
}