using System.Linq;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;
using Pyrite.CoreActions.CoreActions;

namespace Pyrite.CoreActions.CoreAction
{
    public class RunExistingScenarioAction : ICoreAction, IAction
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
            get; set;
        }

        public IAction InputValue { get; set; }

        public string Value
        {
            get
            {
                var scenario = _scenarioRepository.Scenarios.FirstOrDefault(x => x.Id.Equals(TargetScenarioId));
                scenario.OnStateChanged(x => {
                        
                }, true);
                scenario.Execute(InputValue.Value);
            }
            set
            {
                
            }
        }

        public string Caption
        {
            get
            {
                return "Вернуть значение";
            }
        }

        public ActionsDomain.ValueType ValueType
        {
            get
            {
                return InputValue.ValueType;
            }
        }

        public void Initialize()
        {
            //do nothing
        }
    }
}