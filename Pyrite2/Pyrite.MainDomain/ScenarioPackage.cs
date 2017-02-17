using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public class ScenarioPackage
    {
        public string ScenarioId { get; set; } //guid

        public string LastValue { get; set; }

        public AbstractValueType ValueType { get; set; }

        public VisualSettingsBase VisualSettings { get; set; }
    }
}
