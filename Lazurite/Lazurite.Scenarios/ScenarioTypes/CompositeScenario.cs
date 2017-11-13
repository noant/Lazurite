using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.CoreActions.ContextInitialization;
using System.Threading;
using Lazurite.Logging;
using Lazurite.Security;
using Lazurite.Utils;

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
                TargetAction.SetValue(
                    new ExecutionContext(this, param, new OutputChangedDelegates(), cancelToken),
                    string.Empty);
            },
            (exception) => Log.ErrorFormat(exception, "Error while executing scenario [{0}][{1}]", this.Name, this.Id));
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
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
            RaiseEvents();
        }

        public override string GetCurrentValue()
        {
            return _currentValue;
        }

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            foreach (var action in this.TargetAction.GetAllActionsFlat())
            {
                if (action != null)
                {
                    var coreAction = action as ICoreAction;
                    coreAction?.SetTargetScenario(repository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
                    var initializable = action as IContextInitializable;
                    try
                    {
                        initializable?.Initialize(this);
                        action.Initialize();
                    }
                    catch (Exception e)
                    {
                        _log.ErrorFormat(e, "Во время инициализации части сценария [{0}] возникла ошибка", this.Name);
                    }
                }
            }
            return true;
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
