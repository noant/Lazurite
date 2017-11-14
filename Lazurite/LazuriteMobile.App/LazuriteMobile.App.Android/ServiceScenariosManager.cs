using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazuriteMobile.MainDomain;
using Lazurite.MainDomain;
using Java.Lang;

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
                    _callbacks.Dequeue(ServiceOperation.GetClientSettings, Utils.GetData<ClientSettings>(msg));
                    break;
                case ServiceOperation.GetIsConnected:
                    _callbacks.Dequeue(ServiceOperation.GetIsConnected, Utils.GetData<bool>(msg));
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

        public void Initialize(Action<bool> callback)
        {
            if (!IsServiceRunning(typeof(LazuriteService)))
                _activity.StartService(new Intent(Application.Context, typeof(LazuriteService)));
            _serviceConnection = new ManagerServiceConnection();
            _serviceConnection.Connected += ServiceConnection_Connected;
            var result = _activity.BindService(new Intent(Application.Context, typeof(LazuriteService)), _serviceConnection, Bind.AutoCreate);
            if (result)
                _callbacks.Add(ServiceOperation.Initialize, (obj) => callback(true));
            else callback(false);
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

        public void SetClientSettings(ClientSettings settings)
        {
            Utils.SendData(settings, _toServiceMessenger, _messenger, ServiceOperation.SetClientSettings);
        }

        public void GetClientSettings(Action<ClientSettings> callback)
        {
            _callbacks.Add(ServiceOperation.GetClientSettings, (obj) => {
                callback((ClientSettings)obj);
            });
            Utils.SendData(_toServiceMessenger, _messenger, ServiceOperation.GetClientSettings);
        }

        public void IsConnected(Action<bool> callback)
        {
            _callbacks.Add(ServiceOperation.GetIsConnected, (obj) => {
                callback((bool)obj);
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

        private bool IsServiceRunning(System.Type type)
        {
            var manager = (ActivityManager)_activity.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(type).CanonicalName))
                    return true;
            return false;
        }

        public void Close()
        {
            _activity.UnbindService(_serviceConnection);
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

        }

        public event Action<object, Messenger> Connected;
    }
}