using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.CoreActions
{
    [HumanFriendlyName("Выполнить существующий сценарий")]
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(true)]
    [Category(Category.Meta)]
    public class RunExistingScenarioAction : IScenariosAccess, IAction
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
                return false;
            }
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }

        public string Caption
        {
            get
            {
                return _scenario?.Name ?? "[сценарий не выбран]";
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
                return _scenario?.ValueType ?? new ButtonValueType();
            }
            set
            {
                //do nothing
            }
        }

        public RunExistingScenarioMode Mode
        {
            get;
            set;
        } = RunExistingScenarioMode.Synchronously;

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
            string executionId = string.Empty;
            if (Mode == RunExistingScenarioMode.Asynchronously)
                _scenario?.ExecuteAsyncParallel(value, context.CancellationToken);
            else if (Mode == RunExistingScenarioMode.Synchronously)
            {
                context.CancellationToken.Register(() => {
                    if (_scenario.LastExecutionId == executionId)
                        _scenario.TryCancelAll();
                });
                _scenario?.Execute(value, out executionId);
            }
            else if (Mode == RunExistingScenarioMode.MainExecutionContext)
            {
                context.CancellationToken.Register(() => {
                    if (_scenario.LastExecutionId == executionId)
                        _scenario.TryCancelAll();
                });
                _scenario?.ExecuteAsync(value, out executionId);
            }
        }

        public event ValueChangedEventHandler ValueChanged;
    }
}