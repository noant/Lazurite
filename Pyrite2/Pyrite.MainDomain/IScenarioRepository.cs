using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public interface IScenarioRepository
    {
        ScenarioBase[] Scenarios { get; }
    }
}
