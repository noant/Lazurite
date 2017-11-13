using Lazurite.MainDomain;
using System;
using System.Linq;
using System.Threading.Tasks;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using System.Threading;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Security;
using Lazurite.Utils;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Сценарий другой машины")]
    public class RemoteScenario : ScenarioBase
    {
        private IClientFactory _clientFactory;
        private IServer _server;
        private ValueTypeBase _valueType;
        private ScenarioInfo _scenarioInfo;
        private string _currentValue;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Target server ip or name
        /// </summary>
        public string AddressHost { get; set; } = "localhost";

        /// <summary>
        /// Server port
        /// </summary>
        public ushort Port { get; set; } = 8080;

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName { get; set; } = "Lazurite";

        /// <summary>
        /// Server secret key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Target server login
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Target server password
        /// </summary>
        public string Password { get; set; }
        
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
                if (e.ToString().Contains("CommunicationException"))
                {
                    Log.WarnFormat(strErrPrefix + ". Connection error; [{0}][{1}][{2}:{3}/{4}]",
                        this.Name, this.Id, this.AddressHost, this.Port, this.ServiceName);
                }
                if (e.ToString().Contains("EndpointNotFoundException"))
                {
                    Log.WarnFormat(strErrPrefix + ". Endpoint not found; [{0}][{1}][{2}:{3}/{4}]",
                        this.Name, this.Id, this.AddressHost, this.Port, this.ServiceName);
                }
                else if (
                    e.Message.StartsWith("Scenario not found") ||
                    e.Message.StartsWith("Decryption error") ||
                    e.Message.StartsWith("Access denied"))
                {
                    Log.WarnFormat(strErrPrefix + ". " + e.Message+"; [{0}][{1}][{2}:{3}/{4}]",
                        this.Name, this.Id, this.AddressHost, this.Port, this.ServiceName, e.InnerException.Message);
                }
                else
                {
                    Log.WarnFormat(e, strErrPrefix + ". Unrecognized exception; [{0}][{1}][{2}:{3}/{4}]",
                        this.Name, this.Id, this.AddressHost, this.Port, this.ServiceName);
                }
                onException?.Invoke();
                return false;
            }
        }

        public override void Execute(string param, CancellationToken cancelToken)
        {
            HandleExceptions(() => {
                GetServer().ExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
                SetCurrentValueInternal(param);
            });
        }

        public override void ExecuteAsync(string param)
        {
            HandleExceptions(() => {
                GetServer().AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
            });
        }

        public override void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            HandleExceptions(() => {
                GetServer().AsyncExecuteScenarioParallel(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
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
            RaiseEvents();
        }

        public override bool Initialize(ScenariosRepositoryBase repository)
        {
            _clientFactory = Singleton.Resolve<IClientFactory>();
            var initialized = false;
            HandleExceptions(
            () =>
            {
                _scenarioInfo = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
                _valueType = _scenarioInfo.ValueType;
                RemoteScenarioName = _scenarioInfo.Name;
                initialized = true;
            },
            () => {
                initialized = false;
                SetDefaultValue();
            },
            false);
            return Initialized = initialized;
        }

        public override void AfterInitilize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            //changes listener
            TaskUtils.StartLongRunning(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    HandleExceptions(
                    () =>
                    {
                        var newScenInfo = GetServer().GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
                        if (!(newScenInfo.CurrentValue ?? string.Empty).Equals(_currentValue))
                            SetCurrentValueInternal(newScenInfo.CurrentValue ?? string.Empty);
                        this.ValueType = newScenInfo.ValueType;
                        Task.Delay(6500).Wait();
                    },
                    () =>
                    {
                        SetDefaultValue();
                        Task.Delay(200000).Wait();
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
                if (this.ValueType.AcceptedValues.Any())
                    SetCurrentValueInternal(this.ValueType.AcceptedValues[0]);
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
            return _server = _clientFactory.GetServer(AddressHost, Port, ServiceName, SecretKey, UserLogin, Password);
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}