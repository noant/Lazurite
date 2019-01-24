using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Threading;

namespace Lazurite.MainDomain
{
    public abstract class TriggerBase: IAlgorithmContext
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private CancellationTokenSource _tokenSource;
        private string _id = Guid.NewGuid().ToString();
        
        public ValueTypeBase ValueType => _scenario?.ValueType ?? new ButtonValueType();

        protected CancellationToken? CancellationToken => _tokenSource?.Token;

        /// <summary>
        /// Trigger category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Trigger id
        /// </summary>
        public string Id {
            get => _id;
            set => _id = value;
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
            _tokenSource?.Cancel();
            Enabled = false;
        }

        public bool Enabled { get; set; } = true;

        private ScenarioBase _scenario;

        public void SetScenario(ScenarioBase scenario) => _scenario = scenario;

        public ScenarioBase GetScenario() => _scenario;

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
        
        public abstract void Initialize();

        public abstract void AfterInitialize();

        /// <summary>
        /// Begin execute
        /// </summary>
        public void Run()
        {
            try
            {
                if (!_tokenSource?.IsCancellationRequested ?? false)
                    throw new InvalidOperationException("Невозможно запустить триггер, так как предыдущее выполнение не остановлено!");
                Enabled = true;
                _tokenSource = new CancellationTokenSource();
                RunInternal();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Ошибка во время выполнения триггера [{0}][{1}]", Name, Id);
            }
        }

        /// <summary>
        /// Internal run
        /// </summary>
        protected abstract void RunInternal();
    }
}
