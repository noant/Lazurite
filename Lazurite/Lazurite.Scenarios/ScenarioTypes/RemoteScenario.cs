using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using System.Threading;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Logging;

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
            try
            {
                return _currentValue = _server.CalculateScenarioValue(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
            }
            catch (Exception e)
            {
                Log.WarnFormat(e,
                    "Error while calculating remote scenario current value [{0}][{1}][{2} serviceName:{3}]",
                    this.Name, this.Id, this.AddressHost, this.ServiceName);
            }
            return _currentValue;
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            //
        }

        public override void Execute(string param, CancellationToken cancelToken)
        {
            try
            {
                _server.ExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
            }
            catch (Exception e)
            {
                Log.WarnFormat(e, 
                    "Error while executing remote scenario [{0}][{1}][{2} serviceName:{3}]", 
                    this.Name, this.Id, this.AddressHost, this.ServiceName);
            }
        }

        public override void ExecuteAsync(string param)
        {
            try
            {
                _server.AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
            }
            catch (Exception e)
            {
                Log.WarnFormat(e,
                    "Error while async executing remote scenario [{0}][{1}][{2} serviceName:{3}]",
                    this.Name, this.Id, this.AddressHost, this.ServiceName);
            }
        }

        public override void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            try
            {
                _server.AsyncExecuteScenarioParallel(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey));
            }
            catch (Exception e)
            {
                Log.WarnFormat(e,
                    "Error while async parallel executing remote scenario [{0}][{1}][{2} serviceName:{3}]",
                    this.Name, this.Id, this.AddressHost, this.ServiceName);
            }
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
            _cancellationTokenSource = new CancellationTokenSource();
            _clientFactory = Singleton.Resolve<IClientFactory>();
            try
            {
                _server = _clientFactory.GetServer(AddressHost, Port, ServiceName, SecretKey, UserLogin, Password);
                _scenarioInfo = _server.GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
                _valueType = _scenarioInfo.ValueType;
                RemoteScenarioName = _scenarioInfo.Name;
                return true;
            }
            catch
            {
                Log.Warn("Error while initializing remote scenario [" + Name + "]");
                return false;
            }
        }

        public override void AfterInitilize()
        {
            //changes listener
            var task = new Task(() => {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var exceptionThrown = true;
                    try
                    {
                        var newScenInfo = _server.GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
                        if (!newScenInfo.CurrentValue.Equals(_currentValue))
                            SetCurrentValueInternal(newScenInfo.CurrentValue);
                        this.ValueType = newScenInfo.ValueType;
                        exceptionThrown = false;
                    }
                    catch (Exception e)
                    {
                        Log.WarnFormat(e,
                            "Error while listen remote scenario changes [{0}][{1}][{2} serviceName:{3}]",
                            this.Name, this.Id, this.AddressHost, this.ServiceName);
                    }
                    //if connection was failed
                    if (exceptionThrown)
                        Task.Delay(200000).Wait();
                    Task.Delay(4000).Wait();
                }
            },
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning);
            task.Start();
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new IAction[0];
        }

        public IServer GetServer()
        {
            return _server = _clientFactory.GetServer(AddressHost, Port, ServiceName, SecretKey, UserLogin, Password);
        }
    }
}