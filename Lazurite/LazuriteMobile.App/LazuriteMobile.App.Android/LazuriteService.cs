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
using Java.IO;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using Java.Lang;
using LazuriteMobile.Android.ServiceClient;
using Lazurite.Data;
using Lazurite.Logging;
using Lazurite.MainDomain;

namespace LazuriteMobile.App.Droid
{
    [Service(Exported = false, Enabled = true)]
    public class LazuriteService : Service
    {
        public static bool Started { get; private set; } = false;

        private IScenariosManager _manager;
        private Messenger _messenger;
        IncomingHandler _inHandler = new IncomingHandler();
        Messenger _toActivityMessenger;
        
        public override IBinder OnBind(Intent intent)
        {
            return _messenger.Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            if (!Singleton.Any<SaviorBase>())
                Singleton.Add(new JsonFileSavior());
            if (!Singleton.Any<IServiceClientManager>())
                Singleton.Add(new ServiceClientManager());
            if (!Singleton.Any<ILogger>())
                Singleton.Add(new LogStub());
            if (!Singleton.Any<ISystemUtils>())
                Singleton.Add(new SystemUtils());
            _manager = new ScenariosManager();
            _messenger = new Messenger(_inHandler);
            _inHandler.HasCome += InHandler_HasCome;
            _manager.ConnectionLost += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.ConnectionLost);
            _manager.ConnectionRestored += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.ConnectionRestored);
            _manager.LoginOrPasswordInvalid += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.CredentialsInvalid);
            _manager.NeedClientSettings += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.NeedClientSettings);
            _manager.NeedRefresh += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.NeedRefresh);
            _manager.ScenariosChanged += (scenarios) => Utils.RaiseEvent(scenarios, _toActivityMessenger, _messenger, ServiceOperation.ScenariosChanged);
            _manager.SecretCodeInvalid += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.SecretCodeInvalid);
            _manager.ConnectionError += () => Utils.RaiseEvent(_toActivityMessenger, _messenger, ServiceOperation.ConnectionError);
            _manager.Initialize(null);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Started = true;
            Intent activityIntent = new Intent(this, typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, 0,
                activityIntent, PendingIntentFlags.UpdateCurrent);
            
            var notification = 
                new Notification.Builder(this).
                    SetContentTitle("Lazurite запущен").
                    SetContentInfo("...и готов к работе").
                    SetSmallIcon(Resource.Drawable.icon).
                    SetContentIntent(pendingIntent).Build();

            StartForeground(1, notification);
            SetForeground(true);

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            Started = false;
            base.OnDestroy();
        }

        private void InHandler_HasCome(object sender, Message msg)
        {
            _toActivityMessenger = Utils.GetAnswerMessenger(msg);
            switch((ServiceOperation)msg.What)
            {
                case ServiceOperation.ExecuteScenario:
                    {
                        _manager.ExecuteScenario(Utils.GetData<ExecuteScenarioArgs>(msg));
                        break;
                    }
                case ServiceOperation.GetClientSettings:
                    {
                        _manager.GetClientSettings((settings) => {
                            Utils.SendData(settings, _toActivityMessenger, _messenger, ServiceOperation.GetClientSettings);
                        });
                        break;
                    }
                case ServiceOperation.GetIsConnected:
                    {
                        _manager.IsConnected((isConnected) => {
                            Utils.SendData(isConnected, _toActivityMessenger, _messenger, ServiceOperation.GetIsConnected);
                        });
                        break;
                    }
                case ServiceOperation.GetScenarios:
                    {
                        _manager.GetScenarios((scenarios) => {
                            Utils.SendData(scenarios, _toActivityMessenger, _messenger, ServiceOperation.GetScenarios);
                        });
                        break;
                    }
                case ServiceOperation.SetClientSettings:
                    {
                        var settings = Utils.GetData<ConnectionCredentials>(msg);
                        _manager.SetClientSettings(settings);
                        break;
                    }
            }
        }
    }
}