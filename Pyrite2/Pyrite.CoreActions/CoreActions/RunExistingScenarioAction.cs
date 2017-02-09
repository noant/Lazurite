using System.Linq;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;
using Pyrite.CoreActions.CoreActions;
using System;
using System.Threading;

namespace Pyrite.CoreActions.CoreAction
{
    public class RunExistingScenarioAction : ICoreAction, IAction, ISupportsCancellation
    {
        public string TargetScenarioId
        {
            get; set;
        }

        public IAction InputValue { get; set; }

        public string Value
        {
            get
            {
                var scenario = TargetScenario;
                scenario.OnStateChanged(x => {
                    
                }, true);
                scenario.ExecuteAsync(InputValue.Value, CancellationToken);
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
    }
}