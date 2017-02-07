using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;

namespace Pyrite.CoreActions.CoreActions
{
    public class RunExistingScenarioAction : ICoreAction
    {
        private IScenarioRepository _scenarioRepository = Singleton.Resolve<IScenarioRepository>();
        public IScenarioRepository ScenarioRepository
        {
            get
            {
                return _scenarioRepository;
            }
        }

        public string TargetScenarioId
        {
            get;set;
        }

        public IAction InputValue { get; set; }
    }
}
