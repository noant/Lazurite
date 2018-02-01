using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Lazurite.MainDomain
{
    public abstract class ScenarioBase : IAlgorithmContext, IDisposable
    {
        protected static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private List<EventsHandler<ScenarioBase>> _valueChangedEvents = new List<EventsHandler<ScenarioBase>>();
        private List<EventsHandler<ScenarioBase>> _availabilityChangedEvents = new List<EventsHandler<ScenarioBase>>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private bool _isAvailable = false;

        protected void Handle(Exception e)
        {
            Log.ErrorFormat(e, "Error while calculating current value. Scenario: {0}, {1};", Name, Id);
        }

        protected void CheckValue(string param)
        {
            if (!ValueType.Interprete(param).Success)
                throw new InvalidOperationException(string.Format("Value [{0}] is not compatible with value type [{1}]", param, ValueType.GetType().Name));
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
        /// Scenario availability
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            set
            {
                _isAvailable = value;
                RaiseAvailabilityChangedEvents();
                LastChange = DateTime.Now.ToUniversalTime();
            }
        }

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
        public abstract string CalculateCurrentValue();

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public virtual void CalculateCurrentValueAsync(Action<string> callback)
        {
            TaskUtils.Start(
            () => {
                var result = CalculateCurrentValue();
                callback(result);
            },
            (exception) => Handle(exception));
        }

        /// <summary>
        /// Get cached result of scenario execution
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string GetCurrentValue();

        /// <summary>
        /// Set result of scenario execution
        /// </summary>
        public abstract void SetCurrentValueInternal(string value);

        /// <summary>
        /// Method runs after creating of all scenario parameters
        /// </summary>
        public abstract void Initialize(Action<bool> callback = null);
        
        /// <summary>
        /// Method runs after initializing
        /// </summary>
        public abstract void AfterInitilize();

        /// <summary>
        /// Execute scenario in other thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            TaskUtils.StartLongRunning(() => {
                CheckValue(param);
                var output = new OutputChangedDelegates();
                var context = new ExecutionContext(this, param, output, cancelToken);
                HandleExecution(() => ExecuteInternal(context));
            });
        }

        /// <summary>
        /// Execute scenario in main execution context
        /// </summary>
        /// <param name="param"></param>
        public virtual void ExecuteAsync(string param, out string executionId)
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
                    SetCurrentValueInternal(param);
                    ExecuteInternal(context);
                });
            }, Handle);
        }
        
        /// <summary>
        /// Execute in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void Execute(string param, out string executionId)
        {
            CheckValue(param);
            executionId = PrepareExecutionId();
            TryCancelAll();
            var token = PrepareCancellationToken();
            var output = new OutputChangedDelegates();
            output.Add(val => SetCurrentValueInternal(val));
            var context = new ExecutionContext(this, param, output, token);
            HandleExecution(() => {
                SetCurrentValueInternal(param);
                ExecuteInternal(context);
            });
        }

        /// <summary>
        /// Try cancel all operations
        /// </summary>
        public virtual void TryCancelAll()
        {
            _tokenSource?.Cancel();
        }

        protected CancellationToken PrepareCancellationToken()
        {
            _tokenSource = new CancellationTokenSource();
            return _tokenSource.Token;
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
                Log.ErrorFormat(e, "Error while executing scenario [{0}][{1}]", Name, Id);
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
                Log.ErrorFormat(e, "Error while calculating user rights for scenario [{0}][{1}]", Name, Id);
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
                    Log.InfoFormat(e, "Error while raise events in scenario [{1}][{0}]", Name, Id);
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
                    Log.InfoFormat(e, "Error while raise events in scenario [{1}][{0}]", Name, Id);
                }
            }
        }

        public virtual void Dispose()
        {
            Log.DebugFormat("Disposing scenario [{0}][{1}]", Name, Id);
            _valueChangedEvents.Clear();
            _availabilityChangedEvents.Clear();
            TryCancelAll();
        }
    }
}
