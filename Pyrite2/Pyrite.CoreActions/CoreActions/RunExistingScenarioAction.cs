using System.Linq;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;
using System;
using System.Threading;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;

namespace Pyrite.CoreActions.CoreActions
{
    [HumanFriendlyName("ЗапускСценария")]
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    public class RunExistingScenarioAction : ICoreAction, IAction
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
        
        public string Caption
        {
            get
            {
                return _scenario.Name + "(" + ActionsDomain.Utils.ExtractHumanFriendlyName(InputValue.GetType())+ " " + InputValue.Caption + ")";
            }
            set
            {
                //
            }
        }

        public AbstractValueType ValueType
        {
            get
            {
                return _scenario.ValueType;
            }
            set
            {
                //
            }
        }
        
        public RunExistingScenarioMode Mode
        {
            get;
            set;
        }

        public void Initialize()
        {
            //do nothing
        }
        
        public void UserInitializeWith(AbstractValueType valueType)
        {
            //do nothing
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (Mode == RunExistingScenarioMode.Asynchronously)
                _scenario.ExecuteAsyncParallel(value, context.CancellationToken);
            else if (Mode == RunExistingScenarioMode.Synchronously)
                _scenario.Execute(value, context.CancellationToken);
            else if (Mode == RunExistingScenarioMode.MainExecutionContext)
                _scenario.ExecuteAsync(value);
        }
    }
}