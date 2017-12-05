using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Security;
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

        /// <summary>
        /// Execute in current thread
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cancelToken"></param>
        public override void Execute(string param, CancellationToken cancelToken)
        {            
            Log.DebugFormat("Scenario execution begin: [{0}][{1}]", this.Name, this.Id);
            var output = new OutputChangedDelegates();
            output.Add(val => SetCurrentValueInternal(val));
            var context = new ExecutionContext(this, param, output, cancelToken);
            try
            {
                if (!ActionHolder.Action.IsSupportsEvent)
                    SetCurrentValueInternal(param);
                ExecuteInternal(context);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Error while executing scenario [{0}][{1}]", this.Name, this.Id);
            }
            Log.DebugFormat("Scenario execution end: [{0}][{1}]", this.Name, this.Id);
        }

        public override void ExecuteInternal(ExecutionContext context)
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
                _log.ErrorFormat(e, "Во время вычисления значения сценария [{0}] возникла ошибка", this.Name);
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

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            try
            {
                if (this.ActionHolder.Action is ICoreAction && repository != null)
                {
                    ((ICoreAction)ActionHolder.Action)
                        .SetTargetScenario(repository.Scenarios.SingleOrDefault(x => x.Id.Equals(((ICoreAction)ActionHolder.Action).TargetScenarioId)));
                }
                ActionHolder.Action.Initialize();
                _currentValue = ActionHolder.Action.GetValue(null);                
                return true;
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", this.Name);
                return false;
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
