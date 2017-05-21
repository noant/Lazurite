using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;
using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;

namespace LazuriteMobile.App
{
    public class ScenariosManager : IScenariosManager
    {
        private class OperationResult<T> {

            public T Value
            {
                get;
                private set;
            }

            public bool Success
            {
                get;
                private set;
            }

            public OperationResult(T result, bool success)
            {
                Value = result;
                Success = success;
            }
        }

        private IServiceClientManager _clientManager = Singleton.Resolve<IServiceClientManager>();
        private IServiceClient _serviceClient;
        private string _secretKey;

        public ScenarioInfo[] Scenarios
        {
            get; private set;
        }

        public event Action<ScenarioInfo[]> NewScenarios;
        public event Action<ScenarioInfo[]> ScenariosChanged;
        public event Action ConnectionLost;
        public event Action ConnectionRestored;

        public bool Connected { get; private set; }

        private OperationResult<T> Handle<T>(Func<T> func) {
            bool success = false;
            T result = default(T);
            try
            {
                result = func();
                success = true;
            }
            catch
            {
                Connected = false;
                ConnectionLost?.Invoke();
            }
            if (success && !Connected)
                ConnectionRestored?.Invoke();
            return new OperationResult<T>(result, success);
        }

        public void Initialize(string host, ushort port, string serviceName, string login, string password, string secretKey)
        {
            if (_serviceClient != null)
                _serviceClient.Close();

            _secretKey = secretKey;

            _serviceClient = _clientManager.Create(host, port, serviceName, secretKey, login, password);
            _serviceClient.BeginGetScenariosInfo((x) => {
                var result = Handle(() => _serviceClient.EndGetScenariosInfo(x).Decrypt(_secretKey));
                if (result.Success)
                {
                    Scenarios = result.Value.ToArray();
                    NewScenarios?.Invoke(Scenarios);
                }
            }, null);
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void StartListenChanges()
        {
            throw new NotImplementedException();
        }

        public void StopListenChanges()
        {
            throw new NotImplementedException();
        }

        public void ExecuteScenario(string id, string value)
        {
            _serviceClient.BeginAsyncExecuteScenario(new Encrypted<string>(id, _secretKey), new Encrypted<string>(value, _secretKey), null, null);
        }
    }
}
