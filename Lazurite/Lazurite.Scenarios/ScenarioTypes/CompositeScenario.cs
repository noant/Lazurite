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
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase, IStandardValueAction
    {
        public ComplexAction TargetAction { get; set; } = new ComplexAction();

        public override ValueTypeBase ValueType
        {
            get;
            set;
        }

        public override string CalculateCurrentValue(ExecutionContext parentContext) => CalculateCurrentValueInternal();

        public override void CalculateCurrentValueAsync(Action<string> callback, ExecutionContext parentContext) => callback(CalculateCurrentValueInternal());

        protected override string CalculateCurrentValueInternal() => GetCurrentValue();
        
        public override string GetCurrentValue() => _currentValue;

        protected override void ExecuteInternal(ExecutionContext context)
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
            RaiseValueChangedEvents();
        }

        private void InitializeInternal()
        {
            SetInitializationState(ScenarioInitializationValue.Initializing);
            try
            {
                var instanceManager = Singleton.Resolve<IInstanceManager>();

                if (InitializeWithValue == null)
                    InitializeWithValue = ValueType.DefaultValue;
                foreach (var action in TargetAction.GetAllActionsFlat())
                {
                    if (action != null)
                    {
                        instanceManager.PrepareInstance(action, this);
                        action.Initialize();
                    }
                }
                SetIsAvailable(true);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", Name);
                SetIsAvailable(false);
            }
            SetInitializationState(ScenarioInitializationValue.Initialized);
        }

        public override void InitializeAsync(Action<bool> callback)
        {
            InitializeInternal(); //ignore async
            callback?.Invoke(GetIsAvailable());
        }

        public override void AfterInitilize()
        {
            if (ValueType != null && ValueType is ButtonValueType == false) //except buttonValueType because any input value starts scenario permanent
                ExecuteAsync(InitializeWithValue, out string executionId);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
        }

        public override bool FullInitialize()
        {
            InitializeInternal();
            if (GetIsAvailable())
                AfterInitilize();
            return GetIsAvailable();
        }

        public override void FullInitializeAsync(Action<bool> callback = null)
        {
            var result = FullInitialize(); //ignore async
            callback?.Invoke(result);
        }

        public string InitializeWithValue { get; set; }

        //crutch
        public string Value
        {
            get => InitializeWithValue;
            set => InitializeWithValue = value;
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
