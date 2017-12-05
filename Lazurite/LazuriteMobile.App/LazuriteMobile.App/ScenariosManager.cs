using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;

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

        private static readonly int ScenariosManagerListenInterval = GlobalSettings.Get(7500);
        private static readonly int ScenariosManagerListenInterval_onError = GlobalSettings.Get(40000);
        private static readonly int WaitingForRefreshListenInterval = GlobalSettings.Get(120000);
        private static readonly int ScenariosManagerFullRefreshInterval = GlobalSettings.Get(10);
        private static readonly ISystemUtils Utils = Singleton.Resolve<ISystemUtils>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        
        private readonly string _cachedScenariosKey = "scensCache";
        private readonly string _credentialsKey = "credentials";
        private CancellationTokenSource _listenersCancellationTokenSource;
        private CancellationTokenSource _operationCancellationTokenSource;
        private IServiceClientManager _clientManager = Singleton.Resolve<IServiceClientManager>();
        private SaviorBase _savior = Singleton.Resolve<SaviorBase>();
        private IServiceClient _serviceClient;
        private ConnectionCredentials? _credentials;
        private DateTime _lastUpdateTime;
        
        public ScenarioInfo[] Scenarios { get; private set; }
        public bool Connected { get; private set; } = false;
        private bool _succeed = true;
        private int _fullRefreshIncrement = 0;
        private CancellationTokenSource _refreshEndingToken;
        
        public event Action<ScenarioInfo[]> ScenariosChanged;
        public event Action ConnectionLost;
        public event Action ConnectionRestored;
        public event Action NeedRefresh;
        public event Action NeedClientSettings;
        public event Action LoginOrPasswordInvalid;
        public event Action SecretCodeInvalid;
        public event Action CredentialsLoaded;
        public event Action ConnectionError;

        public ScenariosManager()
        {
            //
        }

        private bool HandleExceptions(Action action)
        {
            var cancellationToken = _operationCancellationTokenSource.Token;
            bool success = false;
            try
            {
                action();
                success = true;
                if (!Connected)
                {
                    Connected = true;
                    ConnectionRestored?.Invoke();
                }
            }
            catch (System.Exception e)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    //if login or password wrong; error 403
                    if (e is WebException &&
                        ((HttpWebResponse)((WebException)e).Response)?.StatusCode == HttpStatusCode.Forbidden)
                    {
                        LoginOrPasswordInvalid?.Invoke();
                    }
                    //if data is wrong or secretKey.Length is wrong
                    else if (
                        e is SerializationException
                        || e is DecryptException
                        || e.Message == "Key length not 128/192/256 bits.")
                    {
                        SecretCodeInvalid?.Invoke();
                    }
                    else if (Connected)
                        ConnectionLost?.Invoke();
                    else
                        ConnectionError?.Invoke();
                    Connected = false;
                    success = false;
                }
            }
            if (cancellationToken.IsCancellationRequested)
                return false;
            return success;
        }

        private OperationResult<T> Handle<T>(Func<Encrypted<T>> func) {
            T result = default(T);
            DateTime? serverTime = null;
            var success = HandleExceptions(() =>
            {
                var encryptedResult = func();
                result = encryptedResult.Decrypt(_credentials.Value.SecretKey);
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
                result = encryptedResult.Decrypt(_credentials.Value.SecretKey);
                serverTime = encryptedResult.ServerTime;
            });
            return new OperationResult<List<T>>(result, success, serverTime);
        }
        
        public void Initialize(Action<bool> callback)
        {
            TryLoadClientSettings();
            if (_credentials != null)
                InitializeInternal(callback);
            else 
            {
                callback?.Invoke(false);
                NeedClientSettings?.Invoke();
            }
        }

        private void InitializeInternal(Action<bool> callback)
        {
            //cancel all operations
            _operationCancellationTokenSource?.Cancel();
            _operationCancellationTokenSource = new CancellationTokenSource();
            StopListenChanges();
            RecreateConnection();
            Refresh(callback);
        }

        private void RecreateConnection()
        {
            if (_serviceClient != null)
                _serviceClient.Close();
            _serviceClient = _clientManager.Create(_credentials.Value);
        }
        
        public void StartListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            _listenersCancellationTokenSource = new CancellationTokenSource();
            
            TaskUtils.StartLongRunning(() => {
                while (!_listenersCancellationTokenSource.Token.IsCancellationRequested)
                {
                    RefreshIteration();
                    //sleep while update or refresh
                    Utils.Sleep(WaitingForRefreshListenInterval, _refreshEndingToken.Token);
                    //between updates sleep
                    Utils.Sleep(_succeed ? ScenariosManagerListenInterval : ScenariosManagerListenInterval_onError, CancellationToken.None);
                }
            });
        }

        public void RefreshIteration()
        {
            try
            {
                //recreate connection if error
                if (!_succeed)
                    RecreateConnection();
                _refreshEndingToken = new CancellationTokenSource();
                if (_fullRefreshIncrement == ScenariosManagerFullRefreshInterval || Scenarios == null)
                {
                    Refresh(success =>
                    {
                        _succeed = success;
                        _refreshEndingToken.Cancel();
                    });
                    _fullRefreshIncrement = 0;
                }
                else
                {
                    Update(success =>
                    {
                        _succeed = success;
                        _refreshEndingToken.Cancel();
                    });
                    _fullRefreshIncrement++;
                }
            }
            catch (Exception e)
            {
                Log.Error("Error in listen changes iteration", e);
            }
        }

        public void StopListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            _serviceClient?.Close();
            Connected = false;
        }

        public void ExecuteScenario(ExecuteScenarioArgs args)
        {
            _serviceClient.BeginAsyncExecuteScenario(new Encrypted<string>(args.Id, _credentials.Value.SecretKey), new Encrypted<string>(args.Value, _credentials.Value.SecretKey), 
                (result) => {
                    HandleExceptions(() => _serviceClient.EndAsyncExecuteScenario(result));
                },
            null);
        }

        private void Refresh(Action<bool> callback)
        {
            try
            {
                _serviceClient.BeginGetScenariosInfo((x) =>
                {
                    var result = Handle(() => _serviceClient.EndGetScenariosInfo(x));
                    if (result.Success)
                    {
                        _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                        Scenarios = result.Value.ToArray();
                        NeedRefresh?.Invoke();
                    }
                    callback?.Invoke(result.Success);
                }, null);
            }
            catch
            {
                callback(false);
            }
        }

        public void Refresh()
        {
            Refresh(success => {
                if (!success)
                    RecreateConnection();
            });
        }

        private void Update(Action<bool> callback)
        {
            try
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
                            {
                                existingScenario.CurrentValue = changedScenario.CurrentValue;
                                existingScenario.IsAvailable = changedScenario.IsAvailable;
                            }
                        }
                        _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                        ScenariosChanged?.Invoke(changedScenarios);
                    }
                    callback?.Invoke(result.Success);
                }, null);
            }
            catch
            {
                callback(false);
            }
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
            if (_savior.Has(_credentialsKey))
            {
                try
                {
                    _credentials = _savior.Get<ConnectionCredentials>(_credentialsKey);
                    CredentialsLoaded?.Invoke();
                }
                catch
                {
                    _savior.Clear(_credentialsKey);
                }
            }
        }

        private void SaveClientSettings()
        {
            _savior.Set(_credentialsKey, _credentials);
        }
        
        public void SetClientSettings(ConnectionCredentials credentials)
        {
            _credentials = credentials;
            SaveClientSettings();
            if (_getClientSettingsCallbackCrutch != null)
            {
                _getClientSettingsCallbackCrutch(credentials);
                _getClientSettingsCallbackCrutch = null;
            }
            InitializeInternal(null);
        }

        private Action<ConnectionCredentials> _getClientSettingsCallbackCrutch;
        public void GetClientSettings(Action<ConnectionCredentials> callback)
        {
            if (_credentials == null)
            {
                NeedClientSettings?.Invoke();
                _getClientSettingsCallbackCrutch = callback;
            }
            else
                callback(_credentials.Value);
        }

        public void IsConnected(Action<bool> callback)
        {
            callback(Connected);
        }

        public void GetScenarios(Action<ScenarioInfo[]> callback)
        {
            callback(Scenarios);
        }

        public void Close()
        {
            StopListenChanges();
        }
    }
}
