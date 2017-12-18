using Android.App;
using Android.Content;
using Android.OS;
using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Droid
{
    public class ServiceScenariosManager : IScenariosManager
    {
        private Activity _activity;
        private ManagerServiceConnection _serviceConnection;
        private Messenger _messenger;
        private Messenger _toServiceMessenger;
        private ServiceScenarioManagerCallbacks _callbacks = new ServiceScenarioManagerCallbacks();

        public ServiceScenariosManager(Activity activity)
        {
            _activity = activity;
            var handler = new IncomingHandler();
            handler.HasCome += Handler_HasCome;
            _messenger = new Messenger(handler);
        }
        
        private void Handler_HasCome(object sender, Message msg)
        {
            _toServiceMessenger = Utils.GetAnswerMessenger(msg);
            switch ((ServiceOperation)msg.What)
            {
                case ServiceOperation.GetClientSettings:
                    _callbacks.Dequeue(ServiceOperation.GetClientSettings, Utils.GetData<ConnectionCredentials>(msg));
                    break;
                case ServiceOperation.GetIsConnected:
                    _callbacks.Dequeue(ServiceOperation.GetIsConnected, Utils.GetData<ManagerConnectionState>(msg));
                    break;
                case ServiceOperation.GetScenarios:
                    _callbacks.Dequeue(ServiceOperation.GetScenarios, Utils.GetData<ScenarioInfo[]>(msg));
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

        public event Action ConnectionLost;
        public event Action ConnectionRestored;
        public event Action CredentialsLoaded;
        public event Action LoginOrPasswordInvalid;
        public event Action NeedClientSettings;
        public event Action NeedRefresh;
        public event Action<ScenarioInfo[]> ScenariosChanged;
        public event Action SecretCodeInvalid;
        public event Action ConnectionError;

        public void Initialize(Action<bool> callback)
        {
            if (!LazuriteService.Started)
                Application.Context.StartService(new Intent(Application.Context, typeof(LazuriteService)));
            _serviceConnection = new ManagerServiceConnection();
            _serviceConnection.Connected += ServiceConnection_Connected;
            _serviceConnection.Disconnected += ServiceConnection_Disconnected;
            var result = _activity.BindService(new Intent(Application.Context, typeof(LazuriteService)), _serviceConnection, Bind.AutoCreate);
            if (result && callback != null)
                _callbacks.Add(ServiceOperation.Initialize, (obj) => callback(true));
            else callback?.Invoke(false);
        }

        private void ServiceConnection_Disconnected(ManagerServiceConnection obj)
        {
            Close();
            Initialize((r) => NeedRefresh?.Invoke());
        }

        private void ServiceConnection_Connected(object sender, Messenger msgr)
        {
            _toServiceMessenger = msgr;
            _callbacks.Dequeue(ServiceOperation.Initialize, true);
        }

        public void ExecuteScenario(ExecuteScenarioArgs args)
        {
            Utils.SendData(args, _toServiceMessenger, _messenger, ServiceOperation.ExecuteScenario);
        }

        public void SetClientSettings(ConnectionCredentials creds)
        {
            Utils.SendData(creds, _toServiceMessenger, _messenger, ServiceOperation.SetClientSettings);
        }

        public void GetClientSettings(Action<ConnectionCredentials> callback)
        {
            _callbacks.Add(ServiceOperation.GetClientSettings, (obj) => {
                callback((ConnectionCredentials)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetClientSettings);
        }

        public void IsConnected(Action<ManagerConnectionState> callback)
        {
            _callbacks.Add(ServiceOperation.GetIsConnected, (obj) => {
                callback((ManagerConnectionState)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetIsConnected);
        }

        public void GetScenarios(Action<ScenarioInfo[]> callback)
        {
            _callbacks.Add(ServiceOperation.GetScenarios, (obj) => {
                callback((ScenarioInfo[])obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetScenarios);
        }

        public void ReConnect()
        {
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.ReConnect);
        }

        public void Close()
        {
            _activity.UnbindService(_serviceConnection);
            _serviceConnection.Connected -= ServiceConnection_Connected;
            _serviceConnection.Disconnected -= ServiceConnection_Disconnected;
            _serviceConnection.Dispose();
        }
    }

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
        
        public event Action<ManagerServiceConnection, Messenger> Connected;
        public event Action<ManagerServiceConnection> Disconnected;
    }
}