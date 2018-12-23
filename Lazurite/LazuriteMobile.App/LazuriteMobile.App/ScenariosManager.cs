using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;

namespace LazuriteMobile.App
{
    public class ScenariosManager : IScenariosManager
    {
        private class OperationResult<T>
        {
            public T Value { get; private set; }
            public bool Success { get; private set; }
            public DateTime? ServerTime { get; private set; }

            public OperationResult(T result, bool success, DateTime? serverTime = null)
            {
                Value = result;
                Success = success;
                ServerTime = serverTime;
            }
        }

        private static readonly int ScenariosManagerListenInterval = 16000;
        private static readonly int ScenariosManagerListenInterval_onError = 45000;
        private static readonly int ScenariosManagerFullRefreshInterval = 10;
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly SaviorBase Savior = Singleton.Resolve<SaviorBase>();
        private static readonly AddictionalDataManager Bus = Singleton.Resolve<AddictionalDataManager>();
        private static readonly IClientManager ClientManager = Singleton.Resolve<IClientManager>();
        private static readonly INotifier Notifier = Singleton.Resolve<INotifier>();

        private readonly string _cachedScenariosKey = "scensCache";
        private readonly string _credentialsKey = "credentials";
        private CancellationTokenSource _listenersCancellationTokenSource;
        private CancellationTokenSource _operationCancellationTokenSource;
        private ConnectionCredentials? _credentials;
        private DateTime _lastUpdateTime;
        
        public ScenarioInfo[] Scenarios { get; private set; }
        public ManagerConnectionState ConnectionState { get; private set; } = ManagerConnectionState.Disconnected;
        private bool _succeed = true;
        private int _refreshIncrement = 0;

        public event Action<ScenarioInfo[]> ScenariosChanged;
        public event Action ConnectionLost;
        public event Action ConnectionRestored;
        public event Action NeedRefresh;
        public event Action NeedClientSettings;
        public event Action LoginOrPasswordInvalid;
        public event Action SecretCodeInvalid;
        public event Action CredentialsLoaded;
        public event Action ConnectionError;
        public event Action AccessLocked;

        public ScenariosManager()
        {
            if (!Singleton.Any<IGeolocationListener>())
            {
                var listener = new GeolocationListener();
                Singleton.Add(listener);
                listener.StartListenChanges();
            }

            if (!Bus.Any<GeolocationDataHandler>())
                Bus.Register<GeolocationDataHandler>();
            if (!Bus.Any<DeviceDataHandler>())
                Bus.Register<DeviceDataHandler>();
            if (!Bus.Any<MessagesDataHandler>())
                Bus.Register<MessagesDataHandler>();
        }

        private bool HandleExceptions(Action<IServer> action)
        {
            var cancellationToken = _operationCancellationTokenSource.Token;
            bool success = false;
            try
            {
                if (_credentials != null && ClientManager.IsClosed())
                    RecreateConnection();
                if (ConnectionState == ManagerConnectionState.Disconnected)
                    ConnectionState = ManagerConnectionState.Connecting;
                using (var connection = (IDisposable)ClientManager.GetActualInstance())
                    action((IServer)connection);
                success = true;
                if (ConnectionState != ManagerConnectionState.Connected)
                {
                    ConnectionState = ManagerConnectionState.Connected;
                    ConnectionRestored?.Invoke();
                }
            }
            catch (System.Exception e)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.AccessDenied))
                        LoginOrPasswordInvalid?.Invoke();
                    //if data is wrong or secretKey.Length is wrong
                    else if (
                        SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.DecryptionError)
                        || e is SerializationException
                        || e is DecryptException
                        || e.Message == "Key length not 128/192/256 bits.")
                        SecretCodeInvalid?.Invoke();
                    else if (ConnectionState != ManagerConnectionState.Connected)
                        ConnectionLost?.Invoke();
                    else
                        ConnectionError?.Invoke();
                    ConnectionState = ManagerConnectionState.Disconnected;
                    success = false;
                }
            }

            if (cancellationToken.IsCancellationRequested)
                return false;
            return success;
        }

        private OperationResult<T> Handle<T>(Func<IServer, Encrypted<T>> func) {
            T result = default(T);
            DateTime? serverTime = null;
            var success = HandleExceptions((s) =>
            {
                var encryptedResult = func(s);
                result = encryptedResult.Decrypt(_credentials.Value.SecretKey);
                serverTime = encryptedResult.ServerTime.ToDateTime();
            });
            return new OperationResult<T>(result, success, serverTime);
        }

        private OperationResult<List<T>> Handle<T>(Func<IServer, EncryptedList<T>> func)
        {
            List<T> result = null;
            DateTime? serverTime = null;
            var success = HandleExceptions((s) =>
            {
                var encryptedResult = func(s);
                result = encryptedResult.Decrypt(_credentials.Value.SecretKey);
                serverTime = encryptedResult.ServerTime.ToDateTime();
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

        public void ReConnect()
        {
            RecreateConnection();
            Refresh();
        }

        private void InitializeInternal(Action<bool> callback)
        {
            //cancel all operations
            _operationCancellationTokenSource?.Cancel();
            _operationCancellationTokenSource = new CancellationTokenSource();
            StopListenChanges();
            RecreateConnection();
            Refresh();
        }

        private void RecreateConnection()
        {
            ClientManager.Close();
            ClientManager.CreateConnection(_credentials.Value);
        }

        public void StartListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            _listenersCancellationTokenSource =
                SystemUtils.StartTimer(
                    (token) => RefreshIteration(),
                    () => _succeed ? ScenariosManagerListenInterval : ScenariosManagerListenInterval_onError);
        }

        private bool _iterationRefreshNow;

        public void RefreshIteration()
        {
            if (_iterationRefreshNow)
                return;
            _iterationRefreshNow = true;
            try
            {
                //recreate connection if error
                if (_credentials == null)
                {
                    NeedClientSettings?.Invoke();
                    _iterationRefreshNow = false;
                }
                else
                {
                    if (!_succeed)
                        RecreateConnection();
                    if (SyncAddictionalData())
                    {
                        if (IsMultiples(_refreshIncrement, ScenariosManagerFullRefreshInterval) || Scenarios == null)
                            Refresh();
                        else
                            Update();
                    }
                }
                _refreshIncrement++;
            }
            catch (Exception e)
            {
                Log.Error("Error in listen changes iteration", e);
            }
            _iterationRefreshNow = false;
        }

        private bool IsMultiples(int sum, int num) => (sum >= num && sum % num == 0);

        public void StopListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            ClientManager.Close();
            ConnectionState = ManagerConnectionState.Disconnected;
        }

        public void ExecuteScenario(ExecuteScenarioArgs args)
        {
            TaskUtils.Start(() =>
                HandleExceptions((s) =>
                   s.AsyncExecuteScenario(
                       new Encrypted<string>(args.Id, _credentials.Value.SecretKey),
                       new Encrypted<string>(args.Value, _credentials.Value.SecretKey))));
        }

        private bool Refresh()
        {
            try
            {
                var result = Handle((s) => s.GetScenariosInfo());
                if (_succeed = result.Success)
                {
                    _lastUpdateTime = result.ServerTime ?? _lastUpdateTime;
                    Scenarios = result.Value.ToArray();
                    NeedRefresh?.Invoke();
                }
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        public void RefreshAndRecreate()
        {
            if (!Refresh())
                RecreateConnection();
        }

        private bool Update()
        {
            try
            {
                var result = Handle((s) => s.GetChangedScenarios(SafeDateTime.FromDateTime(_lastUpdateTime)));
                _succeed = result.Success;
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
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        private bool SyncAddictionalData()
        {
            try
            {
                var result = Handle((s) => s.SyncAddictionalData(new Encrypted<AddictionalData>(Bus.Prepare(null), _credentials.Value.SecretKey)));
                _succeed = result.Success;
                if (result.Success && result.Value != null && result.Value.Data.Any())
                    Bus.Handle(result.Value, null);
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        private void CacheScenarios()
        {
            Savior.Set(_cachedScenariosKey, Scenarios);
        }

        private void TryLoadCachedScenarios()
        {
            if (Savior.Has(_cachedScenariosKey))
            {
                try
                {
                    Scenarios = Savior.Get<ScenarioInfo[]>(_cachedScenariosKey);
                    NeedRefresh?.Invoke();
                }
                catch
                {
                    Savior.Clear(_cachedScenariosKey);
                }
            }
        }

        private void TryLoadClientSettings()
        {
            if (Savior.Has(_credentialsKey))
            {
                try
                {
                    _credentials = Savior.Get<ConnectionCredentials>(_credentialsKey);
                    CredentialsLoaded?.Invoke();
                }
                catch
                {
                    Savior.Clear(_credentialsKey);
                }
            }
        }

        private void SaveClientSettings()
        {
            Savior.Set(_credentialsKey, _credentials);
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

        public void IsConnected(Action<ManagerConnectionState> callback)
        {
            callback(ConnectionState);
        }

        public void GetScenarios(Action<ScenarioInfo[]> callback)
        {
            callback(Scenarios);
        }

        public void GetNotifications(Action<LazuriteNotification[]> callback)
        {
            callback(Notifier.GetNotifications());
        }

        public void Close()
        {
            StopListenChanges();
        }

        public void ScreenOnActions()
        {
            //do nothing there
        }
    }
}
