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
        private ILogger _log = Singleton.Resolve<ILogger>();

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
        
        public override void Execute(string param, out string executionId)
        {
            CheckValue(param);
            executionId = PrepareExecutionId();
            TryCancelAll();
            var token = PrepareCancellationToken();
            var output = new OutputChangedDelegates();
            output.Add(val => SetCurrentValueInternal(val));
            var context = new ExecutionContext(this, param, output, token);
            HandleExecution(() => {
                if (!ActionHolder.Action.IsSupportsEvent)
                    SetCurrentValueInternal(param);
                ExecuteInternal(context);
            });
        }
        
        public override void ExecuteAsync(string param, out string executionId)
        {
            CheckValue(param);
            executionId = PrepareExecutionId();
            TaskUtils.StartLongRunning(() => {
                TryCancelAll();
                var token = PrepareCancellationToken();
                var output = new OutputChangedDelegates();
                output.Add(val => SetCurrentValueInternal(val));
                var context = new ExecutionContext(this, param, output, token);
                HandleExecution(() => {
                    if (!ActionHolder.Action.IsSupportsEvent)
                        SetCurrentValueInternal(param);
                    ExecuteInternal(context);
                });
            }, Handle);
        }

        protected override void ExecuteInternal(ExecutionContext context)
        {
            ActionHolder.Action.SetValue(context, context.Input);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return new[] { ActionHolder.Action.GetType() };
        }

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            if (!ActionHolder.Action.IsSupportsEvent)
                base.CalculateCurrentValueAsync(callback);
            //return cached value, callback in not neccesary
            else callback(GetCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            try
            {
                //if action not send some info when value changed then calculate value
                if (!ActionHolder.Action.IsSupportsEvent)
                    return ActionHolder.Action.GetValue(new ExecutionContext(this, string.Empty, new OutputChangedDelegates(), new CancellationToken()));
                //else - cached value is fresh
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время вычисления значения сценария [{0}] возникла ошибка", Name);
            }
            return GetCurrentValue();
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

        public override void Initialize(Action<bool> callback)
        {
            try
            {
                var instanceManager = Singleton.Resolve<IInstanceManager>();
                instanceManager.PrepareInstance(ActionHolder.Action, this);                
                ActionHolder.Action.Initialize();
                _currentValue = ActionHolder.Action.GetValue(null);
                callback?.Invoke(true);
                IsAvailable = true;
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", Name);
                IsAvailable = false;
                callback?.Invoke(false);
            }
        }

        public override void AfterInitilize()
        {
            if (ActionHolder.Action.IsSupportsEvent)
               ActionHolder.Action.ValueChanged += (action, value) => SetCurrentValueInternal(value);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new[] { ActionHolder.Action };
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
