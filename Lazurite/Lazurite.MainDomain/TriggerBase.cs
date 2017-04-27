using Lazurite.ActionsDomain;
using Lazurite.Exceptions;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public abstract class TriggerBase
    {
        public readonly IExceptionsHandler ExceptionsHandler = Singleton.Resolve<IExceptionsHandler>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// Trigger category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Trigger id
        /// </summary>
        public string Id {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        /// <summary>
        /// Trigger name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If value of scenario changed, then trigger body runs
        /// </summary>
        public string TargetScenarioId { get; set; }

        /// <summary>
        /// Trigger body
        /// </summary>
        public IAction TargetAction { get; set; }
        
        /// <summary>
        /// Disable trigger executing
        /// </summary>
        public void Stop()
        {
            _tokenSource.Cancel();
            Enabled = false;
        }

        public bool Enabled { get; set; }

        private ScenarioBase _scenario;

        public void SetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public ScenarioBase GetScenario()
        {
            return _scenario;
        }

        /// <summary>
        /// Get all action types used in target action
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetAllUsedActionTypes();

        /// <summary>
        /// Get all actions used in trigger target action
        /// </summary>
        /// <returns></returns>
        public abstract IAction[] GetAllActionsFlat();

        /// <summary>
        /// Runs on load
        /// </summary>
        public abstract void Initialize(ScenariosRepositoryBase scenariosRepository);

        /// <summary>
        /// Begin execute
        /// </summary>
        protected void Run()
        {
            Enabled = true;
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;
            Task.Factory.StartNew(() => {
                ExceptionsHandler.Handle(this, () => RunInternal(cancellationToken));
            }, 
            cancellationToken, 
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        /// <summary>
        /// Internal run
        /// </summary>
        protected abstract void RunInternal(CancellationToken cancellationToken);
    }
}
