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
        public ScenarioValueChangedEventArgs(ScenarioBase scenario, bool onlyIntent, ScenarioActionSource source, string value, string prevValue)
        {
            Scenario = scenario;
            OnlyIntent = onlyIntent;
            Source = source;
            Value = value;
            PreviousValue = prevValue;
        }

        public ScenarioBase Scenario { get; }
        public bool OnlyIntent { get; }
        public ScenarioActionSource Source { get; }
        public string Value { get; }
        public string PreviousValue { get; }
    }
}
