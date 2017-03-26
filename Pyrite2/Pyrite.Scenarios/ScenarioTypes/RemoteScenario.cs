using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.IOC;
using System.Threading;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.MainDomain.MessageSecurity;

namespace Pyrite.Scenarios.ScenarioTypes
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
        public string AddressHost { get; set; }
        
        /// <summary>
        /// Server port
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName { get; set; }

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
                //
            }
        }

        public override string CalculateCurrentValue()
        {
            _currentValue = _server.CalculateScenarioValue(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
            return _currentValue;
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            //
        }

        public override void Execute(string param, CancellationToken cancelToken)
        {
            ExceptionsHandler.Handle(this, () => _server.ExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey)));
        }

        public override void ExecuteAsync(string param)
        {
            ExceptionsHandler.Handle(this, () => _server.AsyncExecuteScenario(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey)));
        }

        public override void ExecuteAsyncParallel(string param, CancellationToken cancelToken)
        {
            ExceptionsHandler.Handle(this, () => _server.AsyncExecuteScenarioParallel(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(param, SecretKey)));
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

        public override void Initialize(ScenariosRepositoryBase repository)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _clientFactory = Singleton.Resolve<IClientFactory>();
            _server = _clientFactory.GetServer(AddressHost, Port, ServiceName, SecretKey, UserLogin, Password);
            _scenarioInfo = _server.GetScenarioInfo(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey);
            _valueType = _scenarioInfo.ValueType;
            //changes listener
            var task = new Task(() => {                
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var exceptionThrown = true;
                    ExceptionsHandler.Handle(this, () =>
                    {
                        if (_server.IsScenarioValueChanged(new Encrypted<string>(RemoteScenarioId, SecretKey), new Encrypted<string>(_currentValue, SecretKey)))
                            SetCurrentValueInternal(_server.GetScenarioValue(new Encrypted<string>(RemoteScenarioId, SecretKey)).Decrypt(SecretKey));
                        exceptionThrown = false;
                    }, 
                    true);
                    //if connection was failed
                    if (exceptionThrown)
                        Task.Delay(200000);
                    Task.Delay(2000);
                }
            },
            _cancellationTokenSource.Token, 
            TaskCreationOptions.LongRunning);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return new IAction[0];
        }
    }
}
