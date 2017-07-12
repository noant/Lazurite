using System.Linq;
using Lazurite.MainDomain;
using Lazurite.IOC;
using Lazurite.ActionsDomain;
using System;
using System.Threading;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions.CoreActions
{
    [HumanFriendlyName("Запуск сценария")]
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

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
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

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public ValueTypeBase ValueType
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

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
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

        public event ValueChangedDelegate ValueChanged;
    }
}