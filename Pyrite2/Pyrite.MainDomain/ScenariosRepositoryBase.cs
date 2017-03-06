using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public abstract class ScenariosRepositoryBase
    {
        public abstract ScenarioBase[] Scenarios { get; }
        public ScenarioBase[] GetDependentScenarios(Type[] types)
        {
            return Scenarios
                .Where(x => x.GetAllUsedActionTypes()
                .Any(z=>types.Any(y=>y.Equals(z)))).ToArray();
        }
    }
}
