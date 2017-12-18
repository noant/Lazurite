using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Security;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase, IStandardValueAction
    {
        private ILogger _log = Singleton.Resolve<ILogger>();

        public ComplexAction TargetAction { get; set; } = new ComplexAction();

        public override ValueTypeBase ValueType
        {
            get;
            set;
        }

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            //async is unneccesary
            callback(CalculateCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            //just return last "returned" state
            return GetCurrentValue();
        }
        
        public override void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            TaskUtils.StartLongRunning(() =>
            {
                CheckValue(param);
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", this.Name, this.Id);
                TargetAction.SetValue(
                    new ExecutionContext(this, param, new OutputChangedDelegates(), cancelToken),
                    string.Empty);
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", this.Name, this.Id);
            },
            (exception) => Log.ErrorFormat(exception, "Error while executing scenario [{0}][{1}]", this.Name, this.Id));
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            CheckValue(context.Input);
            TargetAction.SetValue(context, string.Empty);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return TargetAction.GetAllActionsFlat().Select(x => x.GetType()).Distinct().ToArray();
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

        public override void Initialize(ScenariosRepositoryBase repository, Action<bool> callback)
        {
            try
            {
                if (InitializeWithValue == null)
                    InitializeWithValue = this.ValueType.DefaultValue;
                foreach (var action in this.TargetAction.GetAllActionsFlat())
                {
                    if (action != null)
                    {
                        var coreAction = action as ICoreAction;
                        coreAction?.SetTargetScenario(repository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
                        var initializable = action as IContextInitializable;
                        initializable?.Initialize(this);
                        action.Initialize();
                    }
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", this.Name);
                this.IsAvailable = false;
            }
            callback?.Invoke(this.IsAvailable);
        }

        public override void AfterInitilize()
        {
            if (this.ValueType != null && this.ValueType is ButtonValueType == false) //except buttonValueType because any input value starts scenario permanent
                ExecuteAsync(InitializeWithValue);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
        }

        public string InitializeWithValue { get; set; }

        //crutch
        public string Value
        {
            get
            {
                return InitializeWithValue;
            }
            set
            {
                InitializeWithValue = value;
            }
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
