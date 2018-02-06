using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
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
        private readonly static int ScenarioListenInterval = GlobalSettings.Get(6500);
        private readonly static int ScenarioListenInterval_onError = GlobalSettings.Get(13000);

        private IClientFactory _clientFactory;
        private IServer _server;
        private ValueTypeBase _valueType;
        private ScenarioInfo _scenarioInfo;
        private string _currentValue;
        private CancellationTokenSource _cancellationTokenSource;

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

        public override string CalculateCurrentValue(ExecutionContext parentContext) => CalculateCurrentValueInternal();

        public override void CalculateCurrentValueAsync(Action<string> callback, ExecutionContext parentContext) => callback(CalculateCurrentValueInternal());
        
        protected override string CalculateCurrentValueInternal() => _currentValue;

        protected override void ExecuteInternal(ExecutionContext context)
        {
            //
        }

        private bool HandleExceptions(Action action, Action onException = null, bool execution = true)
        {
            var strErrPrefix = "Error while remote scenario connection";
            if (execution)
                strErrPrefix = "Error while remote scenario execution";
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

        public override void Execute(string param, out string executionId, ExecutionContext parentContext)
        {
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
            HandleExceptions(() => 
            {
                GetServer().ExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                SetCurrentValueInternal(param);
            });
            Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
        }

        public override void ExecuteAsync(string param, out string executionId, ExecutionContext parentContext)
        {
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            TaskUtils.Start(() =>
            {
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", Name, Id);
                HandleExceptions(() =>
                {
                    GetServer().AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                    SetCurrentValueInternal(param);
                });
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", Name, Id);
            });
        }

        public override void ExecuteAsyncParallel(string param, ExecutionContext parentContext)
        {
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
            _cancellationTokenSource?.Cancel();
            base.TryCancelAll();
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return new Type[] { };
        }

        public override string GetCurrentValue()
        {
            return _currentValue;
        }

        public override void SetCurrentValueInternal(string value)
        {
            _currentValue = value;
            RaiseValueChangedEvents();
        }

        private bool InitializeInternal()
        {
            SetInitializationState(ScenarioInitializationValue.Initializing);
            UpdateValueAsDefault();
            Log.DebugFormat("Scenario initialize begin: [{0}][{1}]", Name, Id);
            _clientFactory = Singleton.Resolve<IClientFactory>();
            _clientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged; //crutch
            _clientFactory.ConnectionStateChanged += ClientFactory_ConnectionStateChanged;
            SetIsAvailable(false);
            var remoteScenarioAvailable = false;
            var initialized = false;
            HandleExceptions(
            () =>
            {
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

        public override void InitializeAsync(Action<bool> callback)
        {
            TaskUtils.Start(() =>
            {
                var result = InitializeInternal();
                callback?.Invoke(result);
            });
        }

        public override void AfterInitilize()
        {
            bool error = false;
            //changes listener
            _cancellationTokenSource = SystemUtils.StartTimer((token) => {
                Log.DebugFormat("Remote scenario refresh iteration begin: [{0}][{1}]", Name, Id);
                HandleExceptions(
                () =>
                {
                    if (string.IsNullOrEmpty(Credentials.SecretKey))
                        throw new InvalidOperationException("Необходим ввод секретного ключа");
                    if (error)
                        ReInitialize();
                    else
                    {
                        var newScenInfo = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey)).Decrypt(Credentials.SecretKey);
                        if (!(newScenInfo.CurrentValue ?? string.Empty).Equals(_currentValue))
                            SetCurrentValueInternal(newScenInfo.CurrentValue ?? string.Empty);
                        ValueType = newScenInfo.ValueType;
                        Log.DebugFormat("Remote scenario refresh iteration end: [{0}][{1}]", Name, Id);
                    }
                    error = false;
                },
                () =>
                {
                    SetIsAvailable(false);
                    Log.DebugFormat("Remote scenario refresh iteration end: [{0}][{1}]", Name, Id);
                    SetDefaultValue();
                    error = true;
                },
                false);
            },
            () => error ? ScenarioListenInterval_onError : ScenarioListenInterval);
        }

        public override bool FullInitialize()
        {
            InitializeInternal();
            if (GetIsAvailable())
                AfterInitilize();
            return GetIsAvailable();
        }

        private void ClientFactory_ConnectionStateChanged(object sender, EventsArgs<bool> args)
        {
            if (((ConnectionStateChangedEventArgs)args).Credentials.Equals(Credentials))
                SetIsAvailable(args.Value);
        }

        private void SetDefaultValue()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            SetCurrentValueInternal(ValueType.DefaultValue);
        }

        private void UpdateValueAsDefault()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            _currentValue = ValueType.DefaultValue;
        }

        public void ReInitialize()
        {
            InitializeInternal();
        }

        public bool InitializedInternal { get; private set; }

        public override IAction[] GetAllActionsFlat()
        {
            return new IAction[0];
        }

        public IServer GetServer()
        {
            return _server = _clientFactory.GetServer(Credentials);
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();

        public override void Dispose()
        {
            if (_clientFactory != null)
                _clientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged;
            base.Dispose();
        }
    }
}