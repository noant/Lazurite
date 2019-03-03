using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using SimpleRemoteMethods.Bases;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class ClientManager : IClientManager
    {
        private class OperationResult<T>
        {
            public T Value { get; private set; }
            public bool Success { get; private set; }

            public OperationResult(T result, bool success)
            {
                Value = result;
                Success = success;
            }
        }

        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();
        private static readonly AddictionalDataManager Bus = Singleton.Resolve<AddictionalDataManager>();
        private static readonly INotifier Notifier = Singleton.Resolve<INotifier>();
        private static readonly StoredPropertiesManager PropertiesManager = Singleton.Resolve<StoredPropertiesManager>();
        private static readonly IGeolocationListener GeolocationListener = Singleton.Resolve<IGeolocationListener>();

        private static readonly string CachedScenariosKey = "scensCache";
        private static readonly string CredentialsKey = "credentials";

        private SafeCancellationToken _listenersCancellationTokenSource;
        private SafeCancellationToken _operationCancellationTokenSource;
        private ConnectionCredentials? _credentials;
        private LazuriteClient _client;
        private DateTime _lastRefresh = DateTime.Now;
        private bool _succeed = true;
        private int _refreshIncrement = 0;
        private bool _iterationRefreshNow;

        public ScenarioInfo[] Scenarios { get; private set; }
        public ManagerConnectionState ConnectionState { get; private set; } = ManagerConnectionState.Disconnected;

        public ListenerSettings ListenerSettings
        {
            get => PropertiesManager.Get(nameof(ListenerSettings), ListenerSettings.Normal1);
            set => PropertiesManager.Set(nameof(ListenerSettings), value);
        }

        public event Action<ScenarioInfo[]> ScenariosChanged;

        public event Action ConnectionLost;

        public event Action ConnectionRestored;

        public event Action NeedRefresh;

        public event Action NeedClientSettings;

        public event Action LoginOrPasswordInvalid;

        public event Action SecretCodeInvalid;

        public event Action CredentialsLoaded;

        public event Action ConnectionError;

        public event Action BruteforceSuspition;

        public ClientManager()
        {
            if (!Bus.Any<GeolocationDataHandler>())
            {
                Bus.Register<GeolocationDataHandler>();
            }

            if (!Bus.Any<DeviceDataHandler>())
            {
                Bus.Register<DeviceDataHandler>();
            }

            if (!Bus.Any<MessagesDataHandler>())
            {
                Bus.Register<MessagesDataHandler>();
            }
        }

        public void Initialize(Action<bool> callback)
        {
            TryLoadClientSettings();
            TryLoadCachedScenarios();
            if (_credentials != null)
            {
                InitializeInternal(callback);
            }
            else
            {
                callback?.Invoke(false);
                NeedClientSettings?.Invoke();
            }
        }

        public void ReConnect()
        {
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
            Refresh();
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
        }

        public void StartListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            _listenersCancellationTokenSource =
                SystemUtils.StartTimer(
                    (token) => RefreshIteration(),
                    () => _succeed ? ListenerSettings.ScreenOnInterval : ListenerSettings.OnErrorInterval);
        }

        public void RefreshIteration()
        {
            if (_iterationRefreshNow)
            {
                return;
            }

            _iterationRefreshNow = true;
            try
            {
                if (_credentials == null)
                {
                    NeedClientSettings?.Invoke();
                    _iterationRefreshNow = false;
                }
                else
                {
                    GeolocationListener.TryStartListenChanges();
                    if (TaskUtils.Wait(SyncAddictionalData()))
                    {
                        bool isMultiples(int sum, int num) => (sum >= num && sum % num == 0);

                        if (isMultiples(_refreshIncrement, 10) || Scenarios == null)
                        {
                            TaskUtils.Wait(Refresh());
                        }
                        else
                        {
                            TaskUtils.Wait(Update());
                        }
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

        public void StopListenChanges()
        {
            _listenersCancellationTokenSource?.Cancel();
            _listenersCancellationTokenSource = null;
            ConnectionState = ManagerConnectionState.Disconnected;
        }

        public void ExecuteScenario(ExecuteScenarioArgs args)
        {
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
            Handle((s) => s.AsyncExecuteScenario(args.Id, args.Value));
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
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
            {
                callback(_credentials.Value);
            }
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
            GeolocationListener.Stop();
        }

        public void Unbind()
        {
            // Do nothing
        }

        public void ScreenOnActions()
        {
            // Do nothing
        }

        private void InitializeInternal(Action<bool> callback)
        {
            Close();

            if (!GeolocationListener.Started)
            {
                GeolocationListener.StartListenChanges();
            }

            _operationCancellationTokenSource = new SafeCancellationToken();
            _client = ServiceClientFactory.Current.GetClient(_credentials.Value);
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
            Refresh().ContinueWith(t => callback?.Invoke(t.Result));
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
        }

        private async Task<bool> HandleExceptions<T>(Func<LazuriteClient, Task<T>> action)
        {
            var cancellationToken = _operationCancellationTokenSource;
            bool success = false;
            try
            {
                if (ConnectionState == ManagerConnectionState.Disconnected)
                {
                    ConnectionState = ManagerConnectionState.Connecting;
                }

                await action(_client);
                success = true;
                if (ConnectionState != ManagerConnectionState.Connected)
                {
                    ConnectionState = ManagerConnectionState.Connected;
                    ConnectionRestored?.Invoke();
                }
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && e.Code == ErrorCode.LoginOrPasswordInvalid)
            {
                LoginOrPasswordInvalid?.Invoke();
                success = false;
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && (e.Code == ErrorCode.UnknownData || e.Code == ErrorCode.DecryptionErrorCode))
            {
                SecretCodeInvalid?.Invoke();
                success = false;
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && e.Code == ErrorCode.BruteforceSuspicion)
            {
                BruteforceSuspition?.Invoke();
                success = false;
            }
            catch (RemoteException) when (!cancellationToken.IsCancellationRequested)
            {
                if (ConnectionState != ManagerConnectionState.Connected)
                {
                    ConnectionLost?.Invoke();
                }
                else
                {
                    ConnectionError?.Invoke();
                }

                success = false;
            }
            catch (Exception)
            {
                // Do nothing
                success = false;
            }

            if (!success)
            {
                ConnectionState = ManagerConnectionState.Disconnected;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            return success;
        }

        private async Task<bool> Handle(Func<LazuriteClient, Task> action)
        {
            var cancellationToken = _operationCancellationTokenSource;
            bool success = false;
            try
            {
                if (ConnectionState == ManagerConnectionState.Disconnected)
                {
                    ConnectionState = ManagerConnectionState.Connecting;
                }

                await action(_client);
                success = true;
                if (ConnectionState != ManagerConnectionState.Connected)
                {
                    ConnectionState = ManagerConnectionState.Connected;
                    ConnectionRestored?.Invoke();
                }
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && e.Code == ErrorCode.LoginOrPasswordInvalid)
            {
                LoginOrPasswordInvalid?.Invoke();
                success = false;
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && (e.Code == ErrorCode.UnknownData || e.Code == ErrorCode.DecryptionErrorCode))
            {
                SecretCodeInvalid?.Invoke();
                success = false;
            }
            catch (RemoteException e) when (!cancellationToken.IsCancellationRequested && e.Code == ErrorCode.BruteforceSuspicion)
            {
                BruteforceSuspition?.Invoke();
                success = false;
            }
            catch (RemoteException) when (!cancellationToken.IsCancellationRequested)
            {
                if (ConnectionState != ManagerConnectionState.Connected)
                {
                    ConnectionLost?.Invoke();
                }
                else
                {
                    ConnectionError?.Invoke();
                }

                success = false;
            }
            catch (Exception)
            {
                // Do nothing
                success = false;
            }

            if (!success)
            {
                ConnectionState = ManagerConnectionState.Disconnected;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            return success;
        }

        private async Task<OperationResult<T>> Handle<T>(Func<LazuriteClient, Task<T>> func)
        {
            T result = default(T);
            var success = await HandleExceptions(async (s) => result = await func(s));
            return new OperationResult<T>(result, success);
        }

        private async Task<bool> Refresh()
        {
            try
            {
                var result = await Handle((s) => s.GetScenariosInfo());
                if (_succeed = result.Success)
                {
                    _lastRefresh = _client.Client.LastCallServerTime;
                    Scenarios = result.Value.ToArray();
                    CacheScenarios();
                    NeedRefresh?.Invoke();
                }
                return result.Success;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in RefreshTask: " + e.Message);
                return false;
            }
        }

        private async Task<bool> Update()
        {
            try
            {
                var result = await Handle((s) => s.GetChangedScenarios(_lastRefresh));
                if (_succeed = result.Success)
                {
                    _lastRefresh = _client.Client.LastCallServerTime;
                }

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
                    ScenariosChanged?.Invoke(changedScenarios);
                }
                return result.Success;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in UpdateTask: " + e.Message);
                return false;
            }
        }

        private async Task<bool> SyncAddictionalData()
        {
            try
            {
                var result = await Handle((s) => s.SyncAddictionalData(Bus.Prepare(null)));
                _succeed = result.Success;
                if (result.Success && (result.Value?.Data.Any() ?? false))
                {
                    Bus.Handle(result.Value, null);
                }

                return result.Success;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in SyncAddictionalDataTask: " + e.Message);
                return false;
            }
        }

        private void CacheScenarios()
        {
            DataManager.Set(CachedScenariosKey, Scenarios);
        }

        private void TryLoadCachedScenarios()
        {
            if (DataManager.Has(CachedScenariosKey))
            {
                try
                {
                    Scenarios = DataManager.Get<ScenarioInfo[]>(CachedScenariosKey);
                    NeedRefresh?.Invoke();
                }
                catch
                {
                    DataManager.Clear(CachedScenariosKey);
                }
            }
        }

        private void TryLoadClientSettings()
        {
            if (DataManager.Has(CredentialsKey))
            {
                try
                {
                    _credentials = DataManager.Get<ConnectionCredentials>(CredentialsKey);
                    CredentialsLoaded?.Invoke();
                }
                catch
                {
                    DataManager.Clear(CredentialsKey);
                }
            }
        }

        private void SaveClientSettings()
        {
            DataManager.Set(CredentialsKey, _credentials);
        }

        public void GetListenerSettings(Action<ListenerSettings> callback)
        {
            callback?.Invoke(ListenerSettings);
        }

        public void SetListenerSettings(ListenerSettings settings)
        {
            ListenerSettings = settings;
        }

        public void GetGeolocationListenerSettings(Action<GeolocationListenerSettings> callback)
        {
            callback?.Invoke(GeolocationListener.ListenerSettings);
        }

        public void SetGeolocationListenerSettings(GeolocationListenerSettings settings)
        {
            GeolocationListener.ListenerSettings = settings;
        }

        public void GetGeolocationAccuracy(Action<int> callback)
        {
            callback?.Invoke(GeolocationListener.AccuracyMeters);
        }

        public void SetGeolocationAccuracy(int accuracyMeters)
        {
            GeolocationListener.AccuracyMeters = accuracyMeters;
        }

        public void ReInitialize()
        {
            ReConnect();
        }
    }
}