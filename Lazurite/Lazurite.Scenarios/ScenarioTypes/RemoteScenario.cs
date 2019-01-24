using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.RemoteScenarioCode;
using Lazurite.Security;
using SimpleRemoteMethods.Bases;
using System;
using ExecutionContext = Lazurite.ActionsDomain.ExecutionContext;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Сценарий другой машины")]
    public class RemoteScenario : ScenarioBase
    {
        private readonly static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private ValueTypeBase _valueType;
        private ScenarioInfo _scenarioInfo;
        private RemoteScenarioInfo _listenerInfo;

        /// <summary>
        /// Target server credentials
        /// </summary>
        public ConnectionCredentials Credentials { get; set; } = ConnectionCredentials.Default;
        
        /// <summary>
        /// Remote scenario guid
        /// </summary>
        public string RemoteScenarioId { get; set; }

        /// <summary>
        /// Remote scenario name
        /// </summary>
        public string RemoteScenarioName { get; set; }

        /// <summary>
        /// Value type of remote scenario
        /// </summary>
        public override ValueTypeBase ValueType
        {
            get => _valueType;
            set => _valueType = value;
        }

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
            //
        }

        private bool HandleExceptions(Action action, Action onException = null)
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (RemoteException e)
            {
                Log.Info($"Ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]: {e.Message}");
                onException?.Invoke();
                return false;
            }
            catch (Exception e)
            {
                Log.Info($"Непредвиденная ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]", e);
                onException?.Invoke();
                return false;
            }
        }

        public override void Execute(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            Log.Debug($"Scenario execution begin: [{Name}][{Id}]");
            HandleExceptions(async () => 
            {
                await GetClient().ExecuteScenario(RemoteScenarioId, param);
                SetCurrentValue(param, source);
            });
            Log.Debug($"Scenario execution end: [{Name}][{Id}]");
        }

        public override void ExecuteAsync(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            Log.DebugFormat($"Scenario execution begin: [{Name}][{Id}]");
            HandleExceptions(async () =>
            {
                await GetClient().AsyncExecuteScenario(RemoteScenarioId, param);
                SetCurrentValue(param, source);
            });
            Log.DebugFormat($"Scenario execution end: [{Name}][{Id}]");
        }

        public override void ExecuteAsyncParallel(ScenarioActionSource source, string param, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            Log.DebugFormat($"Scenario execution begin: [{Name}][{Id}]");
            HandleExceptions(async () => {
                await GetClient().AsyncExecuteScenarioParallel(RemoteScenarioId, param);
            });
            Log.DebugFormat($"Scenario execution end: [{Name}][{Id}]");
        }

        public override void TryCancelAll()
        {
            if (_listenerInfo != null)
                RemoteScenarioChangesListener.Unregister(_listenerInfo);
            base.TryCancelAll();
        }

        public override Type[] GetAllUsedActionTypes() => new Type[0];
        
        protected override bool InitializeInternal()
        {
            base.InitializeInternal();
            SetInitializationState(ScenarioInitializationValue.Initializing);
            UpdateValueAsDefault();
            Log.DebugFormat($"Scenario initialize begin: [{Name}][{Id}]");
            SetIsAvailable(false);
            var remoteScenarioAvailable = false;
            var initialized = false;
            HandleExceptions(
                async () =>
                {
                    if (string.IsNullOrEmpty(Credentials.SecretKey))
                        throw RemoteException.Get(RemoteExceptionData.UnknownData, "Необходим ввод секретного ключа");
                    _scenarioInfo = await GetClient().GetScenarioInfo(RemoteScenarioId);
                    remoteScenarioAvailable = _scenarioInfo.IsAvailable;
                    _valueType = _scenarioInfo.ValueType;
                    RemoteScenarioName = _scenarioInfo.Name;
                    initialized = true;
                },
                () => {
                    initialized = false;
                    SetDefaultValue();
                }
            );
            Log.Debug($"Scenario initialize end: [{Name}][{Id}]");
            SetIsAvailable(initialized && remoteScenarioAvailable);
            SetInitializationState(ScenarioInitializationValue.Initialized);
            return InitializedInternal = initialized;
        }
        
        public override void AfterInitilize()
        {
            //changes listener
            if (_listenerInfo != null)
                RemoteScenarioChangesListener.Unregister(_listenerInfo);
            _listenerInfo = new RemoteScenarioInfo(Name, Credentials, RemoteScenarioId, ValueChanged, AvailabilityChanged);
            RemoteScenarioChangesListener.Register(_listenerInfo);
        }

        public override bool FullInitialize()
        {
            var result = InitializeInternal();
            AfterInitilize(); // Start anyway - it starts listen remote scenario availability
            return result;
        }

        private void ValueChanged(RemoteScenarioValueChangedArgs args)
        {
            _scenarioInfo = args.ScenarioInfo;
            ValueType = _scenarioInfo.ValueType;
            var value = _scenarioInfo.CurrentValue ?? ValueType.DefaultValue;
            if (!value.Equals(GetCurrentValue()))
                SetCurrentValue(value, SystemActionSource);
            if (_scenarioInfo.IsAvailable != GetIsAvailable())
                SetIsAvailable(_scenarioInfo.IsAvailable);
        }

        private void AvailabilityChanged(RemoteScenarioAvailabilityChangedArgs args)
        {
            if (args.IsAvailable != GetIsAvailable())
            {
                SetIsAvailable(args.IsAvailable);
                if (_scenarioInfo != null)
                    _scenarioInfo.IsAvailable = args.IsAvailable;
            }
        }
        
        private void SetDefaultValue()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            SetCurrentValue(ValueType.DefaultValue, SystemActionSource);
        }

        private void UpdateValueAsDefault()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            SetCurrentValueNoEvents(ValueType.DefaultValue);
        }

        public void ReInitialize() => InitializeInternal();

        public bool InitializedInternal { get; private set; }

        public override IAction[] GetAllActionsFlat() => new IAction[0];

        public LazuriteClient GetClient() => ServiceClientFactory.Current.GetClient(Credentials);

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();        
    }
}