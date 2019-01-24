using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Security;
using System;
using System.Linq;
using ExecutionContext = Lazurite.ActionsDomain.ExecutionContext;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase, IStandardValueAction
    {
        public ComplexAction TargetAction { get; set; } = new ComplexAction();

        public override ValueTypeBase ValueType { get; set; }

        public override string CalculateCurrentValue(ScenarioActionSource source, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            return CalculateCurrentValueInternal();
        }

        public override void CalculateCurrentValueAsync(ScenarioActionSource source, Action<string> callback, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            callback(CalculateCurrentValueInternal());
        }

        protected override string CalculateCurrentValueInternal() => GetCurrentValue();
        
        protected override void ExecuteInternal(ExecutionContext context)
        {
            TargetAction.SetValue(context, string.Empty);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return TargetAction.GetAllActionsFlat().Select(x => x.GetType()).Distinct().ToArray();
        }
        
        protected override bool InitializeInternal()
        {
            base.InitializeInternal();
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
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", Name);
                SetIsAvailable(false);
                return false;
            }
            finally
            {
                SetInitializationState(ScenarioInitializationValue.Initialized);
            }
        }

        public override void InitializeAsync(Action<bool> callback)
        {
            InitializeInternal(); //ignore async
            callback?.Invoke(GetIsAvailable());
        }

        public override void AfterInitilize()
        {
            if (ValueType != null && ValueType is ButtonValueType == false) //except buttonValueType because any input value starts scenario permanent
                ExecuteAsync(SystemActionSource, InitializeWithValue, out string executionId);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
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
