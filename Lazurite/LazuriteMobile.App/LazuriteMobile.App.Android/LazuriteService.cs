using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using LazuriteMobile.Android.ServiceClient;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Droid
{
    [Service(Exported = false, Enabled = true)]
    public class LazuriteService : Service
    {
        static LazuriteService()
        {
            ScenariosManagerListenInterval = GlobalSettings.Get(10000);
            Log = Singleton.Resolve<ILogger>();
        }

        public static bool Started
        {
            get
            {
                var currentType = typeof(LazuriteService);
                var currentTypeName = currentType.ToString();
                var manager = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);
                foreach (var service in manager.GetRunningServices(int.MaxValue))
                {
                    if (service.Service.ShortClassName == currentTypeName)
                        return true;
                }
                return false;
            }
        }

        private static readonly int ScenariosManagerListenInterval;
        private static ILogger Log;

        private ScenariosManager _manager;
        private Messenger _messenger;
        private IncomingHandler _inHandler = new IncomingHandler();
        private Messenger _toActivityMessenger;
        private Notification _currentNotification;
        private System.Timers.Timer _timer;
        private PowerManager.WakeLock wakelock;

        public override IBinder OnBind(Intent intent)
        {
            return _messenger.Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            PowerManager pmanager = (PowerManager)GetSystemService(Context.PowerService);
            wakelock = pmanager.NewWakeLock(WakeLockFlags.Partial, "servicewakelock");
            wakelock.SetReferenceCounted(false);
        }
        
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (!Singleton.Any<IServiceClientManager>())
                Singleton.Add(new ServiceClientManager());

            _manager = new ScenariosManager();
            _messenger = new Messenger(_inHandler);
            _inHandler.HasCome += InHandler_HasCome;
            _manager.ConnectionLost += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionLost));
            _manager.ConnectionRestored += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionRestored));
            _manager.LoginOrPasswordInvalid += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.CredentialsInvalid));
            _manager.NeedClientSettings += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.NeedClientSettings));
            _manager.NeedRefresh += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.NeedRefresh));
            _manager.ScenariosChanged += (scenarios) => Handle((messenger) => Utils.RaiseEvent(scenarios, messenger, _messenger, ServiceOperation.ScenariosChanged));
            _manager.SecretCodeInvalid += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.SecretCodeInvalid));
            _manager.ConnectionError += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionError));
            _manager.Initialize(null);

            Intent activityIntent = new Intent(this, typeof(MainActivity));

            PendingIntent showActivityIntent = PendingIntent.GetActivity(Application.Context, 0,
                activityIntent, PendingIntentFlags.UpdateCurrent);
            
            _currentNotification = 
                new Notification.Builder(this).
                    SetContentTitle("Lazurite работает...").
                    SetSmallIcon(Resource.Drawable.icon).
                    SetContentIntent(showActivityIntent).Build();

            AlarmManager manager = (AlarmManager)GetSystemService(AlarmService);
            long triggerAtTime = SystemClock.ElapsedRealtime() + (10 * 60 * 1000);
            Intent onAndroidAvailable = new Intent(this, typeof(BackgroundReceiver));
            PendingIntent startServiceIntent = PendingIntent.GetBroadcast(this, 0, onAndroidAvailable, 0);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                manager.Cancel(startServiceIntent);
                manager.SetAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, triggerAtTime, startServiceIntent);
            }
            else if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat || Build.VERSION.SdkInt == BuildVersionCodes.Lollipop)
            {
                manager.Cancel(startServiceIntent);
                manager.SetExact(AlarmType.ElapsedRealtimeWakeup, triggerAtTime, startServiceIntent);
            }
            
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
                _timer = null;
            }

            _timer = new System.Timers.Timer();
            _timer.Interval = ScenariosManagerListenInterval;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            _timer.AutoReset = true;
            _timer.Start();

            StartForeground(1, _currentNotification);

            return StartCommandResult.Sticky;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _manager.RefreshIteration();
        }

        public override void OnDestroy()
        {
            _currentNotification.Dispose();
            _manager.Close();
            base.OnDestroy();
        }

        private new void Handle(Action<Messenger> action)
        {
            try
            {
                if (_toActivityMessenger != null)
                    action?.Invoke(_toActivityMessenger);
            }
            catch (System.Exception e)
            {
                Log.Warn(null, e);
            }
        }

        private void InHandler_HasCome(object sender, Message msg)
        {
            try
            {
                _toActivityMessenger = Utils.GetAnswerMessenger(msg);
                switch ((ServiceOperation)msg.What)
                {
                    case ServiceOperation.ExecuteScenario:
                        {
                            _manager.ExecuteScenario(Utils.GetData<ExecuteScenarioArgs>(msg));
                            break;
                        }
                    case ServiceOperation.GetClientSettings:
                        {
                            _manager.GetClientSettings((settings) =>
                            {
                                Handle((messenger) => Utils.SendData(settings, messenger, _messenger, ServiceOperation.GetClientSettings));
                            });
                            break;
                        }
                    case ServiceOperation.GetIsConnected:
                        {
                            _manager.IsConnected((isConnected) =>
                            {
                                Handle((messenger) => Utils.SendData(isConnected, messenger, _messenger, ServiceOperation.GetIsConnected));
                            });
                            break;
                        }
                    case ServiceOperation.GetScenarios:
                        {
                            _manager.GetScenarios((scenarios) =>
                            {
                                Handle((messenger) => Utils.SendData(scenarios, _toActivityMessenger, _messenger, ServiceOperation.GetScenarios));
                            });
                            break;
                        }
                    case ServiceOperation.SetClientSettings:
                        {
                            _manager.SetClientSettings(Utils.GetData<ConnectionCredentials>(msg));
                            break;
                        }
                    case ServiceOperation.ReConnect:
                        {
                            _manager.ReConnect();
                            break;
                        }
                }
            }
            catch (System.Exception e)
            {
                Log.Warn("Error on Handler_HasCome", e);
            }
        }
    }
}