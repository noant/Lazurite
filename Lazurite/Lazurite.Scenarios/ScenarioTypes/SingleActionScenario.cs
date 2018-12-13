using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Security;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Одиночный сценарий")]
    public class SingleActionScenario : ScenarioBase
    {
        public ActionHolder ActionHolder { get; set; } = new ActionHolder();

        public override DateTime LastChange
        {
            get
            {
                if (ActionHolder != null &&
                    ActionHolder.Action != null &&
                    !ActionHolder.Action.IsSupportsEvent && 
                    ActionsDomain.Utils.IsOnlyGetValue(ActionHolder.Action.GetType())) //determine when action needs recalculate every time
                    return DateTime.Now.ToUniversalTime();
                return base.LastChange;
            }
            protected set => base.LastChange = value;
        }

        public override ValueTypeBase ValueType
        {
            get => ActionHolder.Action.ValueType;
            set { }
        }

        public override void Execute(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            CheckRights(source, parentContext);
            try
            {
                CheckValue(param, parentContext);
                TryCancelAll();
                var context = PrepareExecutionContext(param, parentContext);
                HandleExecution(() =>
                {
                    if (!ActionHolder.Action.IsSupportsEvent)
                        SetCurrentValue(param, source);
                    else
                        NotifyOnlyIntent(source, param, GetPreviousValue());
                    ExecuteInternal(context);
                });
            }
            catch (Exception e)
            {
                HandleSet(e);
            }
        }
        
        public override void ExecuteAsync(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            CheckRights(source, parentContext);
            TaskUtils.StartLongRunning(() =>
            {
                CheckValue(param, parentContext);
                TryCancelAll();
                var context = PrepareExecutionContext(param, parentContext);
                HandleExecution(() =>
                {
                    if (!ActionHolder.Action.IsSupportsEvent) 
                        SetCurrentValue(param, source);
                    else
                        NotifyOnlyIntent(source, param, GetPreviousValue());
                    ExecuteInternal(context);
                });
            },
            HandleSet);
        }

        protected override void ExecuteInternal(ExecutionContext context)
        {
            ActionHolder.Action.SetValue(context, context.Input);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return new[] { ActionHolder.Action.GetType() };
        }

        public override void CalculateCurrentValueAsync(ScenarioActionSource source, Action<string> callback, ExecutionContext parentContext)
        {
            if (!ActionHolder.Action.IsSupportsEvent)
                base.CalculateCurrentValueAsync(source, callback, parentContext);
            //return cached value, callback in not necessary
            else
            {
                CheckRights(source, parentContext);
                callback(GetCurrentValue());
            }
        }

        protected override string CalculateCurrentValueInternal()
        {
            try
            {
                //if action not send some info when value changed then calculate value
                if (!ActionHolder.Action.IsSupportsEvent)
                {
                    var value = ActionHolder.Action.GetValue(new ExecutionContext(this, string.Empty, string.Empty, new OutputChangedDelegates(), new CancellationTokenSource()));
                    if (GetCurrentValue() != value)
                        SetCurrentValueNoEvents(value);
                    return value;
                }
                //else - cached value is fresh
                return GetCurrentValue();
            }
            catch (Exception e)
            {
                HandleGet(e);
                throw e;
            }
        }
        
        protected override bool InitializeInternal()
        {
            base.InitializeInternal();
            SetInitializationState(ScenarioInitializationValue.Initializing);
            try
            {
                var instanceManager = Singleton.Resolve<IInstanceManager>();
                instanceManager.PrepareInstance(ActionHolder.Action, this);
                ActionHolder.Action.Initialize();
                if (ActionHolder.Action.IsSupportsEvent)
                {
                    ActionHolder.Action.ValueChanged -= OnActionEvent;
                    ActionHolder.Action.ValueChanged += OnActionEvent;
                }
                SetCurrentValue(ActionHolder.Action.GetValue(null), SystemActionSource);
                SetIsAvailable(true);
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", Name);
                SetIsAvailable(false);
                return false;
            }
            finally
            {
                SetInitializationState(ScenarioInitializationValue.Initialized);
            }
        }

        private void OnActionEvent(IAction action, string value)
        {
            SetCurrentValue(value, SystemActionSource);
            SetIsAvailable(true);
        }

        public override void InitializeAsync(Action<bool> callback)
        {
            InitializeInternal(); //ignore async
            callback?.Invoke(GetIsAvailable());
        }

        public override void AfterInitilize()
        {
            //do nothing
        }
        
        public override void FullInitializeAsync(Action<bool> callback = null)
        {
            var result = FullInitialize(); //ignore async
            callback?.Invoke(result);
        }

        public override IAction[] GetAllActionsFlat() => new[] { ActionHolder.Action };

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
