using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Scenarios.RemoteScenarioCode;
using Lazurite.Security;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Сценарий другой машины")]
    public class RemoteScenario : ScenarioBase
    {
        private readonly static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private IClientFactory _clientFactory;
        private IServer _server;
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

        private bool HandleExceptions(Action action, Action onException = null, bool execution = true)
        {
            var strErrPrefix = "Ошибка во время соединения с удаленным сценарием";
            if (execution)
                strErrPrefix = "Ошибка во время выполнения удаленного сценария";
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                //crutch
                if (e is AggregatedCommunicationException)
                {
                    Log.InfoFormat(strErrPrefix + ". {0}; [{1}][{2}][{3}]",
                        e.Message, Name, Id, Credentials.GetAddress());
                }
                else if (
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectNotFound) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.DecryptionError) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectAccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.AccessDenied))
                {
                    Log.InfoFormat(strErrPrefix + ". " + e.Message + "; [{0}][{1}][{2}][{3}]",
                        Name, Id, Credentials.GetAddress(), e.InnerException?.Message);
                }
                else
                {
                    Log.WarnFormat(e, strErrPrefix + ". Unrecognized exception; [{0}][{1}][{2}][{3}]",
                        Name, Id, Credentials.GetAddress(), e.InnerException?.Message);
                }
                onException?.Invoke();
                return false;
            }
        }

        public override void Execute(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
            HandleExceptions(() => 
            {
                GetServer().ExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                SetCurrentValue(param, source);
            });
            Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
        }

        public override void ExecuteAsync(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            TaskUtils.Start(() =>
            {
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
                HandleExceptions(() =>
                {
                    GetServer().AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                    SetCurrentValue(param, source);
                });
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
            });
        }

        public override void ExecuteAsyncParallel(ScenarioActionSource source, string param, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            TaskUtils.Start(() =>
            {
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
                HandleExceptions(() => {
                    GetServer().AsyncExecuteScenarioParallel(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                });
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
            });
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
            Log.DebugFormat("Scenario initialize begin: [{0}][{1}]", Name, Id);
            _clientFactory = Singleton.Resolve<IClientFactory>();
            SetIsAvailable(false);
            var remoteScenarioAvailable = false;
            var initialized = false;
            HandleExceptions(
            () =>
            {
                if (string.IsNullOrEmpty(Credentials.SecretKey))
                    throw new InvalidOperationException("Необходим ввод секретного ключа");
                var encrypted = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey));
                _scenarioInfo = encrypted.Decrypt(Credentials.SecretKey);
                remoteScenarioAvailable = _scenarioInfo.IsAvailable;
                _valueType = _scenarioInfo.ValueType;
                RemoteScenarioName = _scenarioInfo.Name;
                initialized = true;
            },
            () => {
                initialized = false;
                SetDefaultValue();
            },
            false);
            Log.DebugFormat("Scenario initialize end: [{0}][{1}]", Name, Id);
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
            AfterInitilize(); //start anyway - it starts listen remote scenario availability
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

        public override IAction[] GetAllActionsFlat() => 
            new IAction[0];

        public IServer GetServer() => 
            _server = _clientFactory.GetServer(Credentials);

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();        
    }
}