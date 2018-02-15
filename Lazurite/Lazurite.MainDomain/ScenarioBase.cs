using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;

namespace Lazurite.MainDomain
{
    public abstract class ScenarioBase : IAlgorithmContext, IDisposable
    {
        private const int MaxStackValue = 100;
        protected static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private List<EventsHandler<ScenarioBase>> _valueChangedEvents = new List<EventsHandler<ScenarioBase>>();
        private List<EventsHandler<ScenarioBase>> _availabilityChangedEvents = new List<EventsHandler<ScenarioBase>>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private bool _isAvailable = false;
        private string _previousValue;
        private string _currentValue;
        private ScenarioInitializationValue _initializationState = ScenarioInitializationValue.NotInitialized;

        protected void HandleGet(Exception e)
        {
            Log.ErrorFormat(e, "Ошибка во время вычисления значения сценария: {0}, {1};", Name, Id);
        }

        protected void HandleSet(Exception e)
        {
            Log.ErrorFormat(e, "Ошибка во время выполения сценария: {0}, {1};", Name, Id);
        }

        protected void CheckValue(string param, ExecutionContext parentContext)
        {
            try
            {
                if (!ValueType.Interprete(param).Success)
                    throw new InvalidOperationException(string.Format("Значение [{0}] не совместимо с типом значения [{1}]", param, ValueType.GetType().Name));
            }
            catch (Exception e)
            {
                parentContext?.CancelAll(); //stop execution
                throw e;
            }
}

        protected void CheckContext(ExecutionContext context)
        {
            try
            {
                if (context.Find((x) => x.AlgorithmContext is ScenarioBase scenarioBase && scenarioBase.Id == Id) != null) //if true - then it is circular reference
                    throw new ScenarioExecutionException(ScenarioExecutionError.CircularReference);

                if (context.ExecutionNesting >= MaxStackValue)
                    throw new ScenarioExecutionException(ScenarioExecutionError.StackOverflow);
            }
            catch (Exception e)
            {
                context?.CancelAll(); //stop execution
                throw e;
            }
        }

        protected ExecutionContext PrepareExecutionContext(string param, ExecutionContext parentContext)
        {
            var output = new OutputChangedDelegates();
            output.Add(val => SetCurrentValue(val));
            var tokenSource = PrepareCancellationTokenSource();
            ExecutionContext context;
            var prevValue = GetCurrentValue();
            if (parentContext == null)
                context = new ExecutionContext(this, param, prevValue, output, tokenSource);
            else
            {
                context = new ExecutionContext(this, param, prevValue, output, parentContext, tokenSource);
                parentContext.CancellationTokenSource.Token.Register(tokenSource.Cancel);
                CheckContext(context);
            }
            return context;
        }

        /// <summary>
        /// Scenario category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// If true - scenario cannot be executed, just return value
        /// </summary>
        public bool OnlyGetValue { get; set; }

        /// <summary>
        /// Get current scenario state
        /// </summary>
        public ScenarioInitializationValue GetInitializationState() => _initializationState;

        /// <summary>
        /// Get current scenario state
        /// </summary>
        protected void SetInitializationState(ScenarioInitializationValue value) => _initializationState = value;

        /// <summary>
        /// Scenario availability
        /// </summary>
        public bool GetIsAvailable() => _isAvailable;

        /// <summary>
        /// Scenario availability
        /// </summary>
        public void SetIsAvailable(bool value)
        {
            _isAvailable = value;
            RaiseAvailabilityChangedEvents();
            LastChange = DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// previous value
        /// </summary>
        public string GetPreviousValue() => _previousValue;

        /// <summary>
        /// previous value
        /// </summary>
        protected void SetPreviousValue(string value) => _previousValue = value;

        /// <summary>
        /// Scenario name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Scenario id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Identify execution
        /// </summary>
        public string LastExecutionId { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Type of returning value
        /// </summary>
        public abstract ValueTypeBase ValueType { get; set; }

        /// <summary>
        /// Security settings
        /// </summary>
        public abstract SecuritySettingsBase SecuritySettings { get; set; }

        /// <summary>
        /// Time when CurrentValue was changed last time
        /// </summary>
        public virtual DateTime LastChange { get; protected set; } = DateTime.Now.ToUniversalTime();

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        protected abstract string CalculateCurrentValueInternal();

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public virtual string CalculateCurrentValue(ExecutionContext parentContext)
        {
            try
            {
                if (parentContext != null)
                {
                    //empty context, just for stack overflow and circular reference check
                    var context = new ExecutionContext(this, string.Empty, string.Empty, null, parentContext, parentContext.CancellationTokenSource);
                    CheckContext(context);
                }
                return CalculateCurrentValueInternal();
            }
            catch (Exception e)
            {
                HandleGet(e);
                throw e;
            }
        }

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public virtual void CalculateCurrentValueAsync(Action<string> callback, ExecutionContext parentContext)
        {
            TaskUtils.Start(
            () => {
                var result = CalculateCurrentValue(parentContext);
                callback(result);
            },
            HandleGet);
        }

        /// <summary>
        /// Get cached result of scenario execution
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string GetCurrentValue() => _currentValue;

        /// <summary>
        /// Set result of scenario execution
        /// </summary>
        public virtual void SetCurrentValue(string value)
        {
            SetPreviousValue(GetCurrentValue());
            SetCurrentValueNoEvents(value);
            RaiseValueChangedEvents();
        }

        /// <summary>
        /// Set current value witout raising any events
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetCurrentValueNoEvents(string value) => _currentValue = value;
        
        /// <summary>
        /// Internally initialize
        /// </summary>
        protected virtual bool InitializeInternal()
        {
            SetPreviousValue((ValueType ?? new ButtonValueType()).DefaultValue);
            return true;
        }

        /// <summary>
        /// Method runs after creating of all scenario parameters
        /// </summary>
        public virtual void InitializeAsync(Action<bool> callback = null)
        {
            TaskUtils.Start(() =>
            {
                var result = InitializeInternal();
                callback?.Invoke(result);
            });
        }
        
        /// <summary>
        /// Method runs after initializing
        /// </summary>
        public abstract void AfterInitilize();
        
        /// <summary>
        /// Run Initilaize and AfterInitialize method synchronously
        /// </summary>
        /// <returns></returns>
        public virtual bool FullInitialize()
        {
            var result = InitializeInternal();
            if (result)
                AfterInitilize();
            return result;
        }

        /// <summary>
        /// Run Initilaize and AfterInitialize method asynchronously
        /// </summary>
        /// <returns></returns>
        public virtual void FullInitializeAsync(Action<bool> callback = null)
        {
            TaskUtils.Start(() =>
            {
                var result = FullInitialize();
                callback?.Invoke(result);
            });
        }

        /// <summary>
        /// Execute scenario in other thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void ExecuteAsyncParallel(string param, ExecutionContext parentContext)
        {
            TaskUtils.StartLongRunning(() => {
                CheckValue(param, parentContext);
                var output = new OutputChangedDelegates();
                ExecutionContext context;
                var oldVal = GetPreviousValue();
                if (parentContext != null)
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    parentContext.CancellationTokenSource.Token.Register(cancellationTokenSource.Cancel);
                    context = new ExecutionContext(this, param, oldVal, output, parentContext, cancellationTokenSource);
                }
                else
                {
                    context = new ExecutionContext(this,  param, oldVal, output, new CancellationTokenSource());
                }
                CheckContext(context);
                HandleExecution(() => ExecuteInternal(context));
            },
            HandleSet);
        }

        /// <summary>
        /// Execute scenario in main execution context
        /// </summary>
        /// <param name="param"></param>
        public virtual void ExecuteAsync(string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            TaskUtils.StartLongRunning(() => {
                CheckValue(param, parentContext);
                TryCancelAll();
                var context = PrepareExecutionContext(param, parentContext);
                HandleExecution(() => {
                    SetCurrentValue(param);
                    ExecuteInternal(context);
                });
            }, 
            HandleSet);
        }

        /// <summary>
        /// Execute in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void Execute(string param, out string executionId, ExecutionContext parentContext = null)
        {
            executionId = PrepareExecutionId();
            try
            {
                CheckValue(param, parentContext);
                TryCancelAll();
                var context = PrepareExecutionContext(param, parentContext);
                HandleExecution(() =>
                {
                    SetPreviousValue(GetCurrentValue());
                    SetCurrentValue(param);
                    ExecuteInternal(context);
                });
            }
            catch (Exception e)
            {
                HandleSet(e);
            }
        }

        /// <summary>
        /// Try cancel all operations
        /// </summary>
        public virtual void TryCancelAll()
        {
            _tokenSource?.Cancel();
        }

        protected CancellationTokenSource PrepareCancellationTokenSource()
        {
            return _tokenSource = new CancellationTokenSource();
        }

        protected string PrepareExecutionId()
        {
            return LastExecutionId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// All execution operation must executing through this method
        /// </summary>
        /// <param name="action"></param>
        protected void HandleExecution(Action action)
        {
            Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Ошибка выполнения сценария [{0}][{1}]", Name, Id);
            }
            Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
        }

        /// <summary>
        /// Determine that user can execute scenario
        /// </summary>
        /// <param name="user"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool CanExecute(UserBase user, ScenarioStartupSource source)
        {
            try
            {
                if (SecuritySettings == null)
                    throw new NullReferenceException("Security settings is null");
                return SecuritySettings.IsAvailableForUser(user, source);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Ошибка во время выполнения вычисления прав для выполнения сценария [{0}][{1}]", Name, Id);
                return false;
            }
        }

        /// <summary>
        /// Execute scenario in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        protected abstract void ExecuteInternal(ExecutionContext context);

        /// <summary>
        /// Get all types, used in scenario and derived from IAction
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetAllUsedActionTypes();

        /// <summary>
        /// Get all actions used in scenario
        /// </summary>
        /// <returns></returns>
        public abstract IAction[] GetAllActionsFlat();
        
        /// <summary>
        /// Set event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void SetOnStateChanged(EventsHandler<ScenarioBase> action)
        {
            _valueChangedEvents.Add(action);
        }

        /// <summary>
        /// Remove event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnStateChanged(EventsHandler<ScenarioBase> action)
        {
            _valueChangedEvents.Remove(action);
        }

        /// <summary>
        /// Set event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void SetOnAvailabilityChanged(EventsHandler<ScenarioBase> action)
        {
            _availabilityChangedEvents.Add(action);
        }

        /// <summary>
        /// Remove event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnAvailabilityChanged(EventsHandler<ScenarioBase> action)
        {
            _availabilityChangedEvents.Remove(action);
        }

        /// <summary>
        /// Raise events when state changed
        /// </summary>
        protected void RaiseValueChangedEvents()
        {
            LastChange = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < _valueChangedEvents.Count; i++)
            {
                try
                {
                    _valueChangedEvents[i](this, new EventsArgs<ScenarioBase>(this));
                }
                catch(Exception e)
                {
                    Log.InfoFormat(e, "Ошибка во время выполнения событий сценария [{1}][{0}]", Name, Id);
                }
            }
        }

        /// <summary>
        /// Raise events when availability changed
        /// </summary>
        protected void RaiseAvailabilityChangedEvents()
        {
            for (int i = 0; i < _availabilityChangedEvents.Count; i++)
            {
                try
                {
                    _availabilityChangedEvents[i](this, new EventsArgs<ScenarioBase>(this));
                }
                catch (Exception e)
                {
                    Log.InfoFormat(e, "Ошибка во время выполнения событий сценария [{1}][{0}]", Name, Id);
                }
            }
        }

        public virtual void Dispose()
        {
            Log.DebugFormat("Disposing scenario [{0}][{1}]", Name, Id);
            SetInitializationState(ScenarioInitializationValue.NotInitialized);
            _valueChangedEvents.Clear();
            _availabilityChangedEvents.Clear();
            TryCancelAll();
        }
    }
}
