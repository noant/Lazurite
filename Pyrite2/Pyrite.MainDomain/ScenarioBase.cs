using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public abstract class ScenarioBase
    {
        private List<ScenarioStateChangedEvent> _events = new List<ScenarioStateChangedEvent>();

        /// <summary>
        /// Scenario id
        /// </summary>
        public string Id { get; set; } //guid

        /// <summary>
        /// Type of returning value
        /// </summary>
        public ValueType ValueType { get; set; }

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
        public string LastValue { get; private set; }

        public abstract void Execute(string param);
        
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
