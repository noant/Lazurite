using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public class ScenarioInfo
    {
        public string ScenarioId { get; set; } //guid

        public string CurrentValue { get; set; }

        public AbstractValueType ValueType { get; set; }

        public VisualSettingsBase VisualSettings { get; set; }
    }
}
