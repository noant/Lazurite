using System.Linq;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;
using Pyrite.CoreActions.CoreActions;
using System;
using System.Threading;

namespace Pyrite.CoreActions.CoreAction
{
    [VisualInitialization]
    [OnlyGetValue]
    public class RunExistingScenarioAction : ICoreAction, IAction, ISupportsCancellation
    {
        public string TargetScenarioId
        {
            get; set;
        }

        private ScenarioBase _scenario;
        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }

        public IAction InputValue { get; set; }

        public string Value
        {
            get
            {
                var scenario = TargetScenario;
                string outer = null;
                scenario.OnStateChanged(x => outer = x.LastValue, true);
                scenario.ExecuteAsync(InputValue.Value, CancellationToken);
                while (outer == null || !CancellationToken.IsCancellationRequested)
                    MainDomain.Utils.Sleep();
                return outer;
            }
            set
            {
                //do nothing
            }
        }

        public string Caption
        {
            get
            {
                return "Запуск сценария";
            }
        }

        public ActionsDomain.ValueType ValueType
        {
            get
            {
                return InputValue.ValueType;
            }
            set
            {
                //
            }
        }

        public ScenarioBase TargetScenario
        {
            get;
            set;
        }

        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public void Initialize()
        {
            //do nothing
        }

        public void UserInitialize()
        {
            //do nothing
        }
    }
}