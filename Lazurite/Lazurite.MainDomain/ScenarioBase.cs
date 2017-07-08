using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public abstract class ScenarioBase : IAlgorithmContext
    {
        protected static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private List<Action<ScenarioBase>> _events = new List<Action<ScenarioBase>>();        
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private string _id = Guid.NewGuid().ToString();

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
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Type of returning value
        /// </summary>
        public abstract ValueTypeBase ValueType { get; set; }

        /// <summary>
        /// Security settings
        /// </summary>
        public SecuritySettingsBase SecuritySettings { get; set; }

        /// <summary>
        /// Time when CurrentValue was changed last time
        /// </summary>
        public DateTime LastChange { get; private set; } = DateTime.Now.ToUniversalTime();

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public abstract string CalculateCurrentValue();

        /// <summary>
        /// Current value of scenario execution
        /// </summary>
        public virtual void CalculateCurrentValueAsync(Action<string> callback)
        {
            Task.Factory.StartNew(() => {
                var result = CalculateCurrentValue();
                callback(result);
            });
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
        public abstract bool Initialize(ScenariosRepositoryBase repository);
        
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
            Task.Factory.StartNew(() => Execute(param, cancelToken), cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
            Task.Factory.StartNew(() => Execute(param, token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
            var context = new ExecutionContext(this, param, output, cancelToken);
            try
            {
                ExecuteInternal(context);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Error while executing scenario [{0}][{1}]", this.Name, this.Id);
            }
        }

        /// <summary>
        /// Try cancel all operations
        /// </summary>
        public virtual void TryCancelAll()
        {
            _tokenSource.Cancel();
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
        /// Get all actions used in scenario
        /// </summary>
        /// <returns></returns>
        public abstract IAction[] GetAllActionsFlat();
        
        /// <summary>
        /// Set event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void SetOnStateChanged(Action<ScenarioBase> action)
        {
            _events.Add(action);
        }

        /// <summary>
        /// Remove event on state changed
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnStateChanged(Action<ScenarioBase> action)
        {
            _events.Remove(action);
        }

        /// <summary>
        /// Raise events when state changed
        /// </summary>
        protected void RaiseEvents()
        {
            LastChange = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < _events.Count; i++)
            {
                try
                {
                    _events[i](this);
                }
                catch(Exception e)
                {
                    Log.InfoFormat(e, "Error while raise events in scenario [{1}][{0}]", this.Name, this.Id);
                }
            }
        }        
    }
}
