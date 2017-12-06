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
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }

        public override string CalculateCurrentValue()
        {
            return _currentValue;
        }

        public override void ExecuteInternal(ExecutionContext context)
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
                    Log.WarnFormat(strErrPrefix + ". {0}; [{1}][{2}][{3}]",
                        e.Message, this.Name, this.Id, this.Credentials.GetAddress());
                }
                else if (
                    e.Message.StartsWith("Scenario not found") ||
                    e.Message.StartsWith("Decryption error") ||
                    e.Message.StartsWith("Access denied") ||
                    e.Message.Contains("(403)") ||
                    e is InvalidOperationException)
                {
                    Log.WarnFormat(strErrPrefix + ". " + e.Message + "; [{0}][{1}][{2}][{3}]",
                        this.Name, this.Id, this.Credentials.GetAddress(), e.InnerException?.Message);
                }
                else
                {
                    Log.WarnFormat(e, strErrPrefix + ". Unrecognized exception; [{0}][{1}][{2}][{3}]",
                        this.Name, this.Id, this.Credentials.GetAddress(), e.InnerException?.Message);
                }
                onException?.Invoke();
                return false;
            }
        }

        public override void Execute(string param, CancellationToken cancelToken)
        {
            Log.DebugFormat("Scenario execution begin: [{0}][{1}]", this.Name, this.Id);
            HandleExceptions(() => {
                GetServer().ExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                SetCurrentValueInternal(param);
            });
            Log.DebugFormat("Scenario execution end: [{0}][{1}]", this.Name, this.Id);
        }

        public override void ExecuteAsync(string param)
        {
            TaskUtils.Start(() =>
            {
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", this.Name, this.Id);
                HandleExceptions(() =>
                {
                    GetServer().AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                });
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", this.Name, this.Id);
            });
        }

        public override void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            TaskUtils.Start(() =>
            {
                Log.DebugFormat("Scenario execution begin: [{0}][{1}]", this.Name, this.Id);
                HandleExceptions(() => {
                    GetServer().AsyncExecuteScenarioParallel(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey), new Encrypted<string>(param, Credentials.SecretKey));
                });
                Log.DebugFormat("Scenario execution end: [{0}][{1}]", this.Name, this.Id);
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

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            Log.DebugFormat("Scenario initialize begin: [{0}][{1}]", this.Name, this.Id);
            _clientFactory = Singleton.Resolve<IClientFactory>();
            _clientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged; //crutch
            _clientFactory.ConnectionStateChanged += ClientFactory_ConnectionStateChanged;
            IsAvailable = false;
            var remoteScenarioAvailable = false;
            var initialized = false;
            HandleExceptions(
            () =>
            {
                _scenarioInfo = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey)).Decrypt(Credentials.SecretKey);
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
            Log.DebugFormat("Scenario initialize end: [{0}][{1}]", this.Name, this.Id);
            IsAvailable = initialized && remoteScenarioAvailable;
            return Initialized = initialized;
        }

        private void ClientFactory_ConnectionStateChanged(object sender, EventsArgs<bool> args)
        {
            this.IsAvailable = args.Value;
        }

        public override void AfterInitilize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            //changes listener
            TaskUtils.StartLongRunning(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Log.DebugFormat("Remote scenario refresh iteration begin: [{0}][{1}]", this.Name, this.Id);
                    HandleExceptions(
                    () =>
                    {
                        if (string.IsNullOrEmpty(Credentials.SecretKey))
                            throw new InvalidOperationException("Secret key is null");
                        var newScenInfo = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, Credentials.SecretKey)).Decrypt(Credentials.SecretKey);
                        if (!(newScenInfo.CurrentValue ?? string.Empty).Equals(_currentValue))
                            SetCurrentValueInternal(newScenInfo.CurrentValue ?? string.Empty);
                        this.ValueType = newScenInfo.ValueType;
                        Log.DebugFormat("Remote scenario refresh iteration end: [{0}][{1}]", this.Name, this.Id);
                        SystemUtils.Sleep(ScenarioListenInterval, CancellationToken.None);
                    },
                    () =>
                    {
                        Log.DebugFormat("Remote scenario refresh iteration end: [{0}][{1}]", this.Name, this.Id);
                        SetDefaultValue();
                        SystemUtils.Sleep(ScenarioListenInterval_onError, CancellationToken.None);
                        ReInitialize();
                    },
                    false);
                }
            });
        }

        private void SetDefaultValue()
        {
            if (ValueType == null)
            {
                ValueType = new ButtonValueType();
                SetCurrentValueInternal(string.Empty);
            }
            else
            {
                if (this.ValueType.AcceptedValues?.Any() ?? false)
                {
                    if (this.ValueType is ToggleValueType)
                        SetCurrentValueInternal(ToggleValueType.ValueOFF);
                    else
                        SetCurrentValueInternal(this.ValueType.AcceptedValues[0]);
                }
                else SetCurrentValueInternal(string.Empty);
            }
        }

        public void ReInitialize()
        {
            Initialize(null);
        }

        public bool Initialized { get; private set; }

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