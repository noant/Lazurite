using Android.App;
using Android.Content;
using Android.OS;
using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;
using System.Threading;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    public sealed class ServiceConnectionManager : IClientManager, IDisposable
    {
        private Activity _activity;
        private ManagerServiceConnection _serviceConnection;
        private Messenger _messenger;
        private Messenger _toServiceMessenger;
        private ServiceConnectionManagerCallbacks _callbacks = new ServiceConnectionManagerCallbacks();
        private readonly SynchronizationContext _currentContext = SynchronizationContext.Current;

        public ServiceConnectionManager(Activity activity)
        {
            _activity = activity;
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            var handler = new IncomingHandler();
            handler.HasCome += Handler_HasCome;
            _messenger = new Messenger(handler);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private void Handler_HasCome(object sender, Message msg)
        {
            _toServiceMessenger = Utils.GetAnswerMessenger(msg);
            switch ((ServiceOperation)msg.What)
            {
                case ServiceOperation.GetClientSettings:
                    _callbacks.Dequeue(ServiceOperation.GetClientSettings, Utils.GetData<ConnectionCredentials>(msg));
                    break;

                case ServiceOperation.GetListenerSettings:
                    _callbacks.Dequeue(ServiceOperation.GetListenerSettings, Utils.GetData<ListenerSettings>(msg));
                    break;

                case ServiceOperation.GetGeolocationListenerSettings:
                    _callbacks.Dequeue(ServiceOperation.GetGeolocationListenerSettings, Utils.GetData<GeolocationListenerSettings>(msg));
                    break;

                case ServiceOperation.GetGeolocationAccuracy:
                    _callbacks.Dequeue(ServiceOperation.GetGeolocationAccuracy, Utils.GetData<int>(msg));
                    break;

                case ServiceOperation.GetIsConnected:
                    _callbacks.Dequeue(ServiceOperation.GetIsConnected, Utils.GetData<ManagerConnectionState>(msg));
                    break;

                case ServiceOperation.GetScenarios:
                    _callbacks.Dequeue(ServiceOperation.GetScenarios, Utils.GetData<ScenarioInfo[]>(msg));
                    break;

                case ServiceOperation.GetNotifications:
                    _callbacks.Dequeue(ServiceOperation.GetNotifications, Utils.GetData<LazuriteNotification[]>(msg));
                    break;

                case ServiceOperation.ConnectionLost:
                    ConnectionLost?.Invoke();
                    break;

                case ServiceOperation.ConnectionRestored:
                    ConnectionRestored?.Invoke();
                    break;

                case ServiceOperation.CredentialsInvalid:
                    LoginOrPasswordInvalid?.Invoke();
                    break;

                case ServiceOperation.BruteforceSuspition:
                    BruteforceSuspition?.Invoke();
                    break;

                case ServiceOperation.CredentialsLoaded:
                    CredentialsLoaded?.Invoke();
                    break;

                case ServiceOperation.NeedClientSettings:
                    NeedClientSettings?.Invoke();
                    break;

                case ServiceOperation.NeedRefresh:
                    NeedRefresh?.Invoke();
                    break;

                case ServiceOperation.ScenariosChanged:
                    ScenariosChanged?.Invoke(Utils.GetData<ScenarioInfo[]>(msg));
                    break;

                case ServiceOperation.SecretCodeInvalid:
                    SecretCodeInvalid?.Invoke();
                    break;

                case ServiceOperation.ConnectionError:
                    ConnectionError?.Invoke();
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action ConnectionLost;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action ConnectionRestored;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action CredentialsLoaded;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action LoginOrPasswordInvalid;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action NeedClientSettings;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action NeedRefresh;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action<ScenarioInfo[]> ScenariosChanged;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action SecretCodeInvalid;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action ConnectionError;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event Action BruteforceSuspition;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Initialize(Action<bool> callback)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                Application.Context.StartForegroundService(new Intent(Application.Context, typeof(LazuriteService)));
            else
                Application.Context.StartService(new Intent(Application.Context, typeof(LazuriteService)));

            _serviceConnection = new ManagerServiceConnection();
            _serviceConnection.Connected += ServiceConnection_Connected;
            _serviceConnection.Disconnected += ServiceConnection_Disconnected;
            var result = _activity.BindService(new Intent(Application.Context, typeof(LazuriteService)), _serviceConnection, Bind.AutoCreate);
            if (callback != null)
            {
                if (result)
                    _callbacks.Add(ServiceOperation.Initialize, (obj) => callback(true));
                else
                    callback(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void ExecuteScenario(ExecuteScenarioArgs args)
        {
            Utils.SendData(args, _toServiceMessenger, _messenger, ServiceOperation.ExecuteScenario);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetClientSettings(Action<ConnectionCredentials> callback)
        {
            _callbacks.Add(ServiceOperation.GetClientSettings, (obj) =>
            {
                callback((ConnectionCredentials)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetClientSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void SetClientSettings(ConnectionCredentials creds)
        {
            Utils.SendData(creds, _toServiceMessenger, _messenger, ServiceOperation.SetClientSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetListenerSettings(Action<ListenerSettings> callback)
        {
            _callbacks.Add(ServiceOperation.GetListenerSettings, (obj) =>
            {
                callback((ListenerSettings)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetListenerSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void SetListenerSettings(ListenerSettings settings)
        {
            Utils.SendData(settings, _toServiceMessenger, _messenger, ServiceOperation.SetListenerSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void IsConnected(Action<ManagerConnectionState> callback)
        {
            _callbacks.Add(ServiceOperation.GetIsConnected, (obj) =>
            {
                callback((ManagerConnectionState)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetIsConnected);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetScenarios(Action<ScenarioInfo[]> callback)
        {
            _callbacks.Add(ServiceOperation.GetScenarios, (obj) =>
            {
                callback((ScenarioInfo[])obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetScenarios);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetNotifications(Action<LazuriteNotification[]> callback)
        {
            _callbacks.Add(ServiceOperation.GetNotifications, (obj) =>
            {
                callback((LazuriteNotification[])obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetNotifications);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void RefreshIteration()
        {
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.RefreshIteration);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void ScreenOnActions()
        {
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.ScreenOnActions);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetGeolocationListenerSettings(Action<GeolocationListenerSettings> callback)
        {
            _callbacks.Add(ServiceOperation.GetGeolocationListenerSettings, (obj) =>
            {
                callback((GeolocationListenerSettings)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetGeolocationListenerSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void SetGeolocationListenerSettings(GeolocationListenerSettings settings)
        {
            Utils.SendData(settings, _toServiceMessenger, _messenger, ServiceOperation.SetGeolocationListenerSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetGeolocationAccuracy(Action<int> callback)
        {
            _callbacks.Add(ServiceOperation.GetGeolocationAccuracy, (obj) =>
            {
                callback((int)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetGeolocationAccuracy);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void SetGeolocationAccuracy(int accuracyMeters)
        {
            Utils.SendData(accuracyMeters, _toServiceMessenger, _messenger, ServiceOperation.SetGeolocationAccuracy);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void ReConnect()
        {
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.ReConnect);
        }

        // Special handling
        public void ReInitialize()
        {
            Close();

            Utils.DoAfter(() =>
            {
                _currentContext.Post((s) =>
                {
                    InitializeInternal();
                    Initialize((result) =>
                    {
                        if (!result)
                        {
                            ConnectionLost?.Invoke();
                        }
                    });
                }, null);
            },
            500); //Crutch
        }

        private void ServiceConnection_Disconnected(ManagerServiceConnection obj)
        {
            Initialize((r) => NeedRefresh?.Invoke());
        }

        private void ServiceConnection_Connected(object sender, Messenger msgr)
        {
            _toServiceMessenger = msgr;
            _callbacks.Dequeue(ServiceOperation.Initialize, true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Close()
        {
            try
            {
                Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.Close);
                _activity.UnbindService(_serviceConnection);
                Unbind();
            }
            catch
            {
                // Do nothing
            }
        }

        public void Unbind()
        {
            _serviceConnection.Connected -= ServiceConnection_Connected;
            _serviceConnection.Disconnected -= ServiceConnection_Disconnected;
            _messenger?.Dispose();
            _messenger = null;
            _serviceConnection?.Dispose();
            _serviceConnection = null;
        }

        public void Dispose()
        {
            Unbind();
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ManagerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Connected?.Invoke(this, new Messenger(service));
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Disconnected?.Invoke(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ManagerServiceConnection, Messenger> Connected;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ManagerServiceConnection> Disconnected;
    }
}