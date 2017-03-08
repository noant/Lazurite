using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.Exceptions;
using Pyrite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public abstract class ScenarioBase
    {
        private List<ScenarioStateChangedEvent> _events = new List<ScenarioStateChangedEvent>();
        
        public readonly IExceptionsHandler ExceptionsHandler = Singleton.Resolve<IExceptionsHandler>();

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Scenario category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Scenario name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Scenario id
        /// </summary>
        public string Id { get; set; } //guid

        /// <summary>
        /// Type of returning value
        /// </summary>
        public abstract AbstractValueType ValueType { get; set; }

        /// <summary>
        /// Security settings
        /// </summary>
        public SecuritySettingsBase SecuritySettings { get; set; }

        /// <summary>
        /// Time when CurrentValue was changed last time
        /// </summary>
        public DateTime LastChange { get; private set; }

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public abstract string CalculateCurrentValue();

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
        public abstract void Initialize(ScenariosRepositoryBase repository);

        /// <summary>
        /// Execute scenario in other thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            Task.Factory.StartNew(() => Execute(param, cancelToken), cancelToken);
        }

        /// <summary>
        /// Execute scenario in main execution context
        /// </summary>
        /// <param name="param"></param>
        public virtual void ExecuteAsync(string param)
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() => Execute(param, token), token);
        }
        
        /// <summary>
        /// Execute in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public virtual void Execute(string param, CancellationToken cancelToken)
        {
            var output = new OutputChangedDelegates();
            output.Add(val => SetCurrentValueInternal(val));
            var context = new ExecutionContext(param, output, cancelToken);
            ExceptionsHandler.Handle(this, () => ExecuteInternal(context));
        }

        public bool CanExecute(UserBase user, ScenarioStartupSource source)
        {
            if (SecuritySettings == null)
                return true;
            return SecuritySettings.IsAvailableForUser(user, source);
        }

        /// <summary>
        /// Execute scenario in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public abstract void ExecuteInternal(ExecutionContext context);

        /// <summary>
        /// Get all types, used in scenario and derived from IAction
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetAllUsedActionTypes();

        /// <summary>
        /// Set event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void OnStateChanged(Action<ScenarioBase> action)
        {
            OnStateChanged(action, false);
        }

        /// <summary>
        /// Set event on state changed
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onlyOnce"></param>
        public void OnStateChanged(Action<ScenarioBase> action, bool onlyOnce)
        {
            _events.Add(new ScenarioStateChangedEvent(onlyOnce, action));
        }
                
        /// <summary>
        /// Raise events when state changed
        /// </summary>
        public void RaiseEvents()
        {
            LastChange = DateTime.Now;
            for (int i = 0; i <= _events.Count; i++)
            {
                var @event = _events[i];
                ExceptionsHandler.Handle(this, ()=> @event.Action(this));
                if (@event.OnlyOnce)
                    _events.Remove(@event);
            }
        }
        
        private struct ScenarioStateChangedEvent
        {
            public ScenarioStateChangedEvent(bool onlyOnce, Action<ScenarioBase> action)
            {
                OnlyOnce = onlyOnce;
                Action = action;
            }

            public bool OnlyOnce { get; private set; }
            public Action<ScenarioBase> Action { get; private set; }
        } 
    }
}
