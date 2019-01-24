using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
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
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();

        private static readonly ScenarioActionSource ScenarioActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.OtherScenario, ScenarioAction.Execute);

        public string TargetScenarioId
        {
            get; set;
        }

        private ScenarioBase _scenario;
        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
            ValueType = _scenario?.ValueType ?? new ButtonValueType();
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

        public bool IsSupportsModification => true;

        public ValueTypeBase ValueType
        {
            get;
            set;
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
            if (_scenario != null)
            {
                if (_scenario.GetInitializationState() == ScenarioInitializationValue.NotInitialized)
                    _scenario.FullInitialize();
                else while (_scenario.GetInitializationState() == ScenarioInitializationValue.Initializing)
                        SystemUtils.Sleep(100, context.CancellationTokenSource.Token);

                var executionId = string.Empty;
                if (Mode == RunExistingScenarioMode.Asynchronously)
                    _scenario?.ExecuteAsyncParallel(ScenarioActionSource, value, context);
                else if (Mode == RunExistingScenarioMode.Synchronously)
                    _scenario?.Execute(ScenarioActionSource, value, out executionId, context);
                else if (Mode == RunExistingScenarioMode.MainExecutionContext)
                    _scenario?.ExecuteAsync(ScenarioActionSource, value, out executionId, context);
            }
        }

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
    }
}