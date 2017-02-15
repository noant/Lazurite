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
    public abstract class ScenarioBase : ISupportsCancellation
    {
        private List<ScenarioStateChangedEvent> _events = new List<ScenarioStateChangedEvent>();
        
        private readonly IExceptionsHandler _exceptionsHandler = Singleton.Resolve<IExceptionsHandler>();

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private string _lastValue;

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
        public ActionsDomain.ValueTypes.ValueType ValueType { get; set; }

        /// <summary>
        /// Security settings
        /// </summary>
        public SecuritySettingsBase SecuritySettings { get; set; }

        /// <summary>
        /// Time when LastValue was changed last time
        /// </summary>
        public DateTime DateTimeScenarioChanged { get; private set; }

        /// <summary>
        /// Last result of scenarion executing
        /// </summary>
        public string LastValue {
            get {
                return _lastValue;
            }
            set {
                _lastValue = value;
                RaiseEvents();
            }
        }

        /// <summary>
        /// Токен отмена операции
        /// </summary>
        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public void ExecuteAsync(string param, CancellationToken cancelToken)
        {
            _tokenSource.Cancel();
            var task = new Task(() => {
                Execute(param, cancelToken);
            }, cancelToken);
            task.Start();
        }

        public void ExecuteAsync(string param)
        {
            var cancelToken = _tokenSource.Token;
            ExecuteAsync(param, cancelToken);
        }

        public abstract void Execute(string param, CancellationToken cancelToken);
        
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
        private void RaiseEvents()
        {
            for (int i = 0; i <= _events.Count; i++)
            {
                var @event = _events[i];
                _exceptionsHandler.Handle(this, ()=> @event.Action(this));
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
