using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.Utils;
using System;
using System.Threading;

namespace Lazurite.MainDomain
{
    public abstract class TriggerBase: IAlgorithmContext
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _previousCancellationToken;
        private string _id = Guid.NewGuid().ToString();
        
        public ValueTypeBase ValueType
        {
            get {
                return _scenario?.ValueType ?? new ButtonValueType();
            }
        }

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
        public virtual IAction TargetAction { get; set; }
        
        /// <summary>
        /// Disable trigger executing
        /// </summary>
        public virtual void Stop()
        {
            _tokenSource.Cancel();
            Enabled = false;
        }

        public bool Enabled { get; set; } = true;

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
        
        public abstract void Initialize(ScenariosRepositoryBase scenariosRepository);

        public abstract void AfterInitialize();

        /// <summary>
        /// Begin execute
        /// </summary>
        public void Run()
        {
            Enabled = true;
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;
            TaskUtils.StartLongRunning(
                () => RunInternal(cancellationToken),
                (exception) => Log.ErrorFormat(exception, "Error while starting trigger [{0}][{1}]", this.Name, this.Id));
        }

        /// <summary>
        /// Internal run
        /// </summary>
        protected abstract void RunInternal(CancellationToken cancellationToken);
    }
}
