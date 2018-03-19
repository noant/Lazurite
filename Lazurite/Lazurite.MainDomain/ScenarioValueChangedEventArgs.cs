using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class ScenarioValueChangedEventArgs
    {
        public ScenarioValueChangedEventArgs(ScenarioBase scenario, bool onlyIntent, ScenarioActionSource source)
        {
            Scenario = scenario;
            OnlyIntent = onlyIntent;
            Source = source;
        }

        public ScenarioBase Scenario { get; }
        public bool OnlyIntent { get; }
        public ScenarioActionSource Source { get; }
    }
}
