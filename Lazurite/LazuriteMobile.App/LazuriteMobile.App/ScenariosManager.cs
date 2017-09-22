using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;
using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using System.Threading;
using Xamarin.Forms;
using Lazurite.Data;
using System.ServiceModel;
using System.Net;
using System.Runtime.Serialization;

namespace LazuriteMobile.App
{
    public class ScenariosManager : IScenariosManager
    {
        private class OperationResult<T>
        {
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

            public DateTime? ServerTime
            {
                get;
                private set;
            }

            public OperationResult(T result, bool success, DateTime? serverTime = null)
            {
                Value = result;
                Success = success;
                ServerTime = serverTime;
            }
        }

        private readonly int _listenInterval = 7000;
        private readonly int _fullRefreshInterval = 120;
        private readonly string _cachedScenariosKey = "scensCache";
        private readonly string _clientSettingsKey = "clientSettings";
        private CancellationTokenSource _listenersCancellationTokenSource;
        private CancellationTokenSource _operationCancellationTokenSource;
        private IServiceClientManager _clientManager = Singleton.Resolve<IServiceClientManager>();
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private IServiceClient _serviceClient;
        private ClientSettings _clientSettings;
        private DateTime _lastUpdateTime;

        public ScenarioInfo[] Scenarios { get; private set; }
        public bool Connected { get; private set; }

        public event Action<ScenarioInfo[]> ScenariosChanged;
        public event Action ConnectionLost;
        public event Action ConnectionRestored;
        public event Action NeedRefresh;
        public event Action NeedClientSettings;
        public event Action LoginOrPasswordInvalid;
        public event Action SecretCodeInvalid;
        public event Action CredentialsLoaded;

        public ScenariosManager()
        {
            this.SecretCodeInvalid += () => StopListenChanges();
            this.LoginOrPasswordInvalid += () => StopListenChanges();
        }

        private bool HandleExceptions(Action action)
        {
            var cancellationToken = _operationCancellationTokenSource.Token;
            bool success = false;
            try
            {
                action();
                success = true;
                Connected = true;
            }
            catch (Exception e)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    //if login or password wrong; error 403
                    if (e is WebException && 
                        ((HttpWebResponse)((WebException)e).Response).StatusCode == HttpStatusCode.Forbidden)
                    {
                        LoginOrPasswordInvalid?.Invoke();
                    }
                    //if data is wrong or secretKey.Length is wrong
                    else if (e is SerializationException || e.Message == "Key length not 128/192/256 bits.")
                    {
                        SecretCodeInvalid?.Invoke();
                    }
                    else if (Connected)
                        ConnectionLost?.Invoke();
                    Connected = false;
                    success = false;
                }
            }
            if (cancellationToken.IsCancellationRequested)
                return false;
            if (success && !Connected)
                ConnectionRestored?.Invoke();
            return success;
        }

        private OperationResult<T> Handle<T>(Func<Encrypted<T>> func) {
            T result = default(T);
            DateTime? serverTime = null;
            var success = HandleExceptions(() =>
            {
                var encryptedResult = func();
                result = encryptedResult.Decrypt(_clientSettings.SecretKey);
                serverTime = encryptedResult.ServerTime;
            });
            return new OperationResult<T>(result, success, serverTime);
        }

        private OperationResult<List<T>> Handle<T>(Func<EncryptedList<T>> func)
        {
            List<T> result = null;
            DateTime? serverTime = null;
            var success = HandleExceptions(() =>
            {
                var encryptedResult = func();
                result = encryptedResult.Decrypt(_clientSettings.SecretKey);
                serverTime = encryptedResult.ServerTime;
            });
            return new OperationResult<List<T>>(result, success, serverTime);
        }
        
        public void Initialize()
        {
            TryLoadClientSettings();
            if (_clientSettings != null)
                Initialize(_clientSettings.Host, _clientSettings.Port, _clientSettings.ServiceName, _clientSettings.Login, _clientSettings.Password, _clientSettings.SecretKey);
            else NeedClientSettings?.Invoke();
        }

        private void Initialize(string host, ushort port, string serviceName, string login, string password, string secretKey)
        {
            //cancel all operations
            if (_operationCancellationTokenSource != null)
                _operationCancellationTokenSource.Cancel();
            _operationCancellationTokenSource = new CancellationTokenSource();

            StopListenChanges();

            if (_serviceClient != null)
                _serviceClient.Close();
            
            _serviceClient = _clientManager.Create(host, port, serviceName, secretKey, login, password);
            Connected = true;
            _serviceClient.BeginGetScenariosInfo((x) =>
            {
                var result = Handle(() => _serviceClient.EndGetScenariosInfo(x));
                if (result.Success)
                {
                    _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                    Scenarios = result.Value.ToArray();
                    CacheScenarios();
                    NeedRefresh?.Invoke();
                    StartListenChanges();
                }
            },
            null);
        }
        
        public void StartListenChanges()
        {
            if (_listenersCancellationTokenSource != null)
                _listenersCancellationTokenSource.Cancel();

            _listenersCancellationTokenSource = new CancellationTokenSource();

            int fullRefreshIncrement = 0;

            Device.StartTimer(TimeSpan.FromMilliseconds(_listenInterval), () => {
                var token = _listenersCancellationTokenSource.Token;
                if (token.IsCancellationRequested)
                    return false;
                if (fullRefreshIncrement == _fullRefreshInterval)
                {
                    fullRefreshIncrement = 0;
                    Refresh();
                }
                else
                {
                    _serviceClient.BeginGetChangedScenarios(_lastUpdateTime,
                    (o) =>
                    {
                        var result = Handle(() => _serviceClient.EndGetChangedScenarios(o));
                        if (result.Success && result.Value != null && result.Value.Any())
                        {
                            var changedScenariosLW = result.Value;
                            var changedScenarios = Scenarios.Where(x => changedScenariosLW.Any(z => z.ScenarioId == x.ScenarioId)).ToArray();
                            foreach (var changedScenario in changedScenariosLW)
                            {
                                var existingScenario = changedScenarios.FirstOrDefault(x => x.ScenarioId.Equals(changedScenario.ScenarioId));
                                if (existingScenario != null)
                                    existingScenario.CurrentValue = changedScenario.CurrentValue;
                            }
                            _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                            ScenariosChanged?.Invoke(changedScenarios);
                        }
                    },
                    null);
                    fullRefreshIncrement++;
                }
                return true;
            });
        }

        public void StopListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
        }

        public void ExecuteScenario(string id, string value)
        {
            _serviceClient.BeginAsyncExecuteScenario(new Encrypted<string>(id, _clientSettings.SecretKey), new Encrypted<string>(value, _clientSettings.SecretKey), 
                (result) => {
                    HandleExceptions(() => _serviceClient.EndAsyncExecuteScenario(result));
                }, 
            null);
        }

        public void Refresh()
        {
            _serviceClient.BeginGetScenariosInfo((x) => {
                var result = Handle(() => _serviceClient.EndGetScenariosInfo(x));
                if (result.Success)
                {
                    _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                    Scenarios = result.Value.ToArray();
                    CacheScenarios();
                    NeedRefresh?.Invoke();
                }
            }, null);
        }

        private void CacheScenarios()
        {
            _savior.Set(_cachedScenariosKey, Scenarios);
        }

        private void TryLoadCachedScenarios()
        {
            if (_savior.Has(_cachedScenariosKey))
            {
                try
                {
                    Scenarios = _savior.Get<ScenarioInfo[]>(_cachedScenariosKey);
                    NeedRefresh?.Invoke();
                }
                catch
                {
                    _savior.Clear(_cachedScenariosKey);
                }
            }
        }

        private void TryLoadClientSettings()
        {
            if (_savior.Has(_clientSettingsKey))
            {
                try
                {
                    _clientSettings = _savior.Get<ClientSettings>(_clientSettingsKey);
                    CredentialsLoaded?.Invoke();
                }
                catch
                {
                    _savior.Clear(_clientSettingsKey);
                }
            }
        }

        private void SaveClientSettings()
        {
            _savior.Set(_clientSettingsKey, _clientSettings);
        }

        public ClientSettings GetClientSettings()
        {
            return _clientSettings ?? new ClientSettings();
        }

        public void SetClientSettings(ClientSettings settings)
        {
            _clientSettings = settings;
            SaveClientSettings();
            Initialize(_clientSettings.Host, _clientSettings.Port, _clientSettings.ServiceName, _clientSettings.Login, _clientSettings.Password, _clientSettings.SecretKey);
        }
    }
}
