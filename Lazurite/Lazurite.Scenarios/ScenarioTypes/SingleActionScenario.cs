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
                    return DateTime.Now;
                return base.LastChange;
            }
            protected set => base.LastChange = value;
        }

        public override ValueTypeBase ValueType
        {
            get
            {
                return ActionHolder.Action.ValueType;
            }
            set
            {
                ///
            }
        }
        
        public override void Execute(string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            try
            {
                CheckValue(param, parentContext);
                TryCancelAll();
                var output = new OutputChangedDelegates();
                output.Add(val => SetCurrentValueInternal(val));
                ExecutionContext context;
                if (parentContext == null)
                    context = new ExecutionContext(this, param, output, PrepareCancellationTokenSource());
                else
                {
                    context = new ExecutionContext(this, param, output, parentContext);
                    CheckContext(context);
                }
                HandleExecution(() =>
                {
                    if (!ActionHolder.Action.IsSupportsEvent)
                        SetCurrentValueInternal(param);
                    ExecuteInternal(context);
                });
            }
            catch (Exception e)
            {
                HandleSet(e);
            }
        }
        
        public override void ExecuteAsync(string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            TaskUtils.StartLongRunning(() => {
                CheckValue(param, parentContext);
                TryCancelAll();
                var output = new OutputChangedDelegates();
                output.Add(val => SetCurrentValueInternal(val));
                ExecutionContext context;
                if (parentContext == null)
                    context = new ExecutionContext(this, param, output, PrepareCancellationTokenSource());
                else
                {
                    context = new ExecutionContext(this, param, output, parentContext);
                    CheckContext(context);
                }
                HandleExecution(() => {
                    if (!ActionHolder.Action.IsSupportsEvent)
                        SetCurrentValueInternal(param);
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

        public override void CalculateCurrentValueAsync(Action<string> callback, ExecutionContext parentContext)
        {
            if (!ActionHolder.Action.IsSupportsEvent)
                base.CalculateCurrentValueAsync(callback, parentContext);
            //return cached value, callback in not neccesary
            else callback(GetCurrentValue());
        }

        protected override string CalculateCurrentValueInternal()
        {
            try
            {
                //if action not send some info when value changed then calculate value
                if (!ActionHolder.Action.IsSupportsEvent)
                    return ActionHolder.Action.GetValue(new ExecutionContext(this, string.Empty, new OutputChangedDelegates(), new CancellationTokenSource()));
                //else - cached value is fresh
                return GetCurrentValue();
            }
            catch (Exception e)
            {
                HandleGet(e);
                throw e;
            }
        }

        private string _currentValue;
        public override void SetCurrentValueInternal(string value)
        {
            _currentValue = value;
            RaiseValueChangedEvents();
        }

        public override string GetCurrentValue()
        {
            return _currentValue;
        }

        private void InitializeInternal()
        {
            SetInitializationState(ScenarioInitializationValue.Initializing);
            try
            {
                var instanceManager = Singleton.Resolve<IInstanceManager>();
                instanceManager.PrepareInstance(ActionHolder.Action, this);
                ActionHolder.Action.Initialize();
                _currentValue = ActionHolder.Action.GetValue(null);
                SetIsAvailable(true);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", Name);
                SetIsAvailable(false);
            }
            SetInitializationState(ScenarioInitializationValue.Initialized);
        }

        public override void InitializeAsync(Action<bool> callback)
        {
            InitializeInternal(); //ignore async
            callback?.Invoke(GetIsAvailable());
        }

        public override void AfterInitilize()
        {
            if (ActionHolder.Action.IsSupportsEvent)
               ActionHolder.Action.ValueChanged += (action, value) => SetCurrentValueInternal(value);
        }

        public override bool FullInitialize()
        {
            InitializeInternal();
            if (GetIsAvailable())
                AfterInitilize();
            return GetIsAvailable();
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new[] { ActionHolder.Action };
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
