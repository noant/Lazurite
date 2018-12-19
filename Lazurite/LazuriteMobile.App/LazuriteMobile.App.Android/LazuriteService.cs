using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;
using System.Threading;

namespace LazuriteMobile.App.Droid
{
    [Service(Exported = false, Enabled = true)]
    public class LazuriteService : Service
    {
        private static readonly int SleepInterval_Normal = 10000;
        private static readonly int SleepInterval_ScreenOff = 60000;
        private static readonly int SleepInterval_PowerSaving = 120000;
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private static bool AlreadyStarted = false;

        private bool IsPhoneSleeping
        {
            get
            {
                var service = ((PowerManager)GetSystemService(Service.PowerService));
                return !service.IsInteractive || service.IsDeviceIdleMode;
            }
        }
        private bool IsPhoneInPowerSave
        {
            get
            {
                var service = ((PowerManager)GetSystemService(Service.PowerService));
                return service.IsPowerSaveMode && !service.IsIgnoringBatteryOptimizations(PackageName);
            }
        }

        private static ILogger Log;
        private ScenariosManager _manager;
        private Messenger _messenger;
        private IncomingHandler _inHandler = new IncomingHandler();
        private Messenger _toActivityMessenger;
        private Notification _currentNotification;
        private CancellationTokenSource _timerCancellationToken;
        private PowerManager.WakeLock _wakelock;

        public override IBinder OnBind(Intent intent)
        {
            return _messenger.Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            var pmanager = (PowerManager)GetSystemService(PowerService);
            _wakelock = pmanager.NewWakeLock(WakeLockFlags.Partial, "servicewakelock");
            _wakelock.SetReferenceCounted(false);
        }
        
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (!AlreadyStarted)
            {
                AlreadyStarted = true;
                SingletonPreparator.Initialize();
                MainApplication.InitializeUnhandledExceptionsHandler();
                Log = Singleton.Resolve<ILogger>();

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

                RegisterReceiver(new ScreenOnReciever(), new IntentFilter(Intent.ActionScreenOn));

                var manager = (AlarmManager)GetSystemService(AlarmService);
                var triggerAtTime = SystemClock.ElapsedRealtime() + (10 * 60 * 1000);
                var onAndroidAvailable = new Intent(this, typeof(BackgroundReceiver));
                var startServiceIntent = PendingIntent.GetBroadcast(this, 0, onAndroidAvailable, 0);
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

                ReInitTimer();

                var activityIntent = new Intent(this, typeof(MainActivity));

                var showActivityIntent = PendingIntent.GetActivity(Application.Context, 0,
                    activityIntent, PendingIntentFlags.UpdateCurrent);

                _currentNotification =
                    new Notification.Builder(this).
                        SetContentTitle("Lazurite работает...").
                        SetSmallIcon(Resource.Drawable.icon).
                        SetContentIntent(showActivityIntent).
                        SetVisibility(NotificationVisibility.Private).
                        SetColor(Color.Argb(0, 255, 255, 255).ToArgb()).
                        SetOnlyAlertOnce(true).
                        Build();

                StartForeground(1, _currentNotification);
            }
            return StartCommandResult.Sticky;
        }
        
        private void ReInitTimer()
        {
            _timerCancellationToken?.Cancel();

            _timerCancellationToken = SystemUtils.StartTimer(
                (token) => _manager.RefreshIteration(),
                () => {
                    var interval = SleepInterval_Normal;
                    try
                    {
                        if (IsPhoneSleeping)
                            interval = SleepInterval_ScreenOff;
                        else if (IsPhoneInPowerSave)
                            interval = SleepInterval_PowerSaving;
                    }
                    catch
                    {
                        //do nothing
                    }
                    return interval;
                },
                false);
        }

        public override void OnDestroy()
        {
            AlreadyStarted = false;
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

        public void RefreshIteration() => _manager.RefreshIteration();

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
                    case ServiceOperation.GetNotifications:
                        {
                            _manager.GetNotifications((notifications) =>
                                Handle((messenger) => Utils.SendData(notifications, _toActivityMessenger, _messenger, ServiceOperation.GetNotifications)));
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
                    case ServiceOperation.RefreshIteration:
                        {
                            _manager.RefreshIteration();
                            break;
                        }
                    case ServiceOperation.ScreenOnActions:
                        {
                            _manager.ScreenOnActions();
                            ReInitTimer();
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