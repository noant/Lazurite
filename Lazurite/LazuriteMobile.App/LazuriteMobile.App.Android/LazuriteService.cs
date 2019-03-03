using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using System;
using System.Threading;

namespace LazuriteMobile.App.Droid
{
    [Service(Exported = false, Enabled = true)]
    public class LazuriteService : Service
    {
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static LazuriteService Current;
        private static bool AlreadyStarted = false;
        private static readonly DateTime LastTimerTickTime = DateTime.Now;
        private static ScreenOnReciever ScreenOnReciever;
        private static StartLazuriteServiceReceiver StartLazuriteServiceReceiver;
        private static GpsOnReciever GpsOnReciever;
        private static ILogger Log;

        private PowerManager PowerManager => GetSystemService(PowerService) as PowerManager;

        private bool IsPhoneSleeping => !PowerManager.IsInteractive || PowerManager.IsDeviceIdleMode;

        private bool IsPhoneInPowerSave => PowerManager.IsPowerSaveMode && !PowerManager.IsIgnoringBatteryOptimizations(PackageName);

        private ClientManager _manager;
        private Messenger _messenger;
        private IncomingHandler _inHandler = new IncomingHandler();
        private Messenger _toActivityMessenger;
        private Notification _currentNotification;
        private SafeCancellationToken _timerCancellationToken;
        private PowerManager.WakeLock _wakelock;

        public override IBinder OnBind(Intent intent)
        {
            return _messenger.Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            AlreadyStarted = false;

            Current = this;

            SafeUnregisterReceiver(ScreenOnReciever);
            SafeUnregisterReceiver(StartLazuriteServiceReceiver);
            SafeUnregisterReceiver(GpsOnReciever);
            RegisterReceiver(ScreenOnReciever = new ScreenOnReciever(), new IntentFilter(Intent.ActionScreenOn));
            RegisterReceiver(StartLazuriteServiceReceiver = new StartLazuriteServiceReceiver(), new IntentFilter(Intent.ActionBootCompleted));
            RegisterReceiver(GpsOnReciever = new GpsOnReciever(), new IntentFilter("android.location.GPS_ENABLED_CHANGE"));

            SingletonPreparator.Initialize();
            MainApplication.InitializeUnhandledExceptionsHandler();
            Log = Singleton.Resolve<ILogger>();

            _manager = new ClientManager();

            if (_manager.ListenerSettings.UseCPUInBackground)
            {
                InitWakeLock();
            }

            _messenger = new Messenger(_inHandler);
            _inHandler.HasCome += InHandler_HasCome;

            _manager.ConnectionLost += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionLost));
            _manager.ConnectionRestored += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionRestored), TimerAction.Start);
            _manager.LoginOrPasswordInvalid += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.CredentialsInvalid), TimerAction.Stop);
            _manager.BruteforceSuspition += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.BruteforceSuspition), TimerAction.Stop);
            _manager.NeedClientSettings += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.NeedClientSettings));
            _manager.NeedRefresh += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.NeedRefresh));
            _manager.ScenariosChanged += (scenarios) => Handle((messenger) => Utils.RaiseEvent(scenarios, messenger, _messenger, ServiceOperation.ScenariosChanged));
            _manager.SecretCodeInvalid += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.SecretCodeInvalid));
            _manager.ConnectionError += () => Handle((messenger) => Utils.RaiseEvent(messenger, _messenger, ServiceOperation.ConnectionError));
            _manager.Initialize(null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (!AlreadyStarted)
            {
                AlreadyStarted = true;

                ReInitTimer();

                var activityIntent = new Intent(this, typeof(MainActivity));
                var showActivityIntent = PendingIntent.GetActivity(Application.Context, 0, activityIntent, PendingIntentFlags.UpdateCurrent);
                _currentNotification =
#pragma warning disable CS0618 // Тип или член устарел
                    new Notification.Builder(this).
#pragma warning restore CS0618 // Тип или член устарел
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private void ReInitTimer()
        {
            _timerCancellationToken?.Cancel();

            _timerCancellationToken = SystemUtils.StartTimer(
                (token) => _manager.RefreshIteration(),
                () =>
                {
                    if (_manager == null)
                    {
                        return Timeout.Infinite;
                    }

                    var interval = _manager.ListenerSettings.ScreenOnInterval;
                    try
                    {
                        if (IsPhoneSleeping)
                        {
                            interval = _manager.ListenerSettings.ScreenOffInterval;
                        }
                        else if (IsPhoneInPowerSave)
                        {
                            interval = _manager.ListenerSettings.PowerSavingModeInterval;
                        }
                        else if (_manager.ConnectionState != MainDomain.ManagerConnectionState.Connected)
                        {
                            interval = _manager.ListenerSettings.OnErrorInterval;
                        }
                    }
                    catch
                    {
                        //do nothing
                    }
                    return interval;
                },
                false);
        }

        private void StopTimer()
        {
            _timerCancellationToken?.Cancel();
        }

        private bool IsTimerStarted => !_timerCancellationToken?.IsCancellationRequested ?? false;

        private void SafeUnregisterReceiver(BroadcastReceiver receiver)
        {
            if (receiver == null)
            {
                return;
            }

            try
            {
                UnregisterReceiver(receiver);
            }
            catch
            {
                // Ignore, receiver already unregistered
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private new void Handle(Action<Messenger> action, TimerAction timerAction = TimerAction.Nothing)
        {
            switch (timerAction)
            {
                case TimerAction.Start:
                    if (!IsTimerStarted)
                    {
                        ReInitTimer();
                    }

                    break;

                case TimerAction.Stop:
                    StopTimer();
                    break;

                case TimerAction.Nothing:
                    break;
            }
            try
            {
                if (_toActivityMessenger != null)
                {
                    action?.Invoke(_toActivityMessenger);
                }
            }
            catch (System.Exception e)
            {
                Log.Warn(null, e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public void RefreshIteration() => _manager.RefreshIteration();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
                                Handle((messenger) => Utils.SendData(scenarios, messenger, _messenger, ServiceOperation.GetScenarios));
                            });
                            break;
                        }
                    case ServiceOperation.GetNotifications:
                        {
                            _manager.GetNotifications((notifications) =>
                                Handle((messenger) => Utils.SendData(notifications, messenger, _messenger, ServiceOperation.GetNotifications)));
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
                    case ServiceOperation.GetListenerSettings:
                        {
                            _manager.GetListenerSettings((settings) =>
                            {
                                Handle((messenger) => Utils.SendData(settings, messenger, _messenger, ServiceOperation.GetListenerSettings));
                            });
                            break;
                        }
                    case ServiceOperation.SetListenerSettings:
                        {
                            var settings = Utils.GetData<ListenerSettings>(msg);
                            _manager.SetListenerSettings(settings);
                            break;
                        }
                    case ServiceOperation.GetGeolocationAccuracy:
                        {
                            _manager.GetGeolocationAccuracy((acc) =>
                                Handle((messenger) => Utils.SendData(acc, messenger, _messenger, ServiceOperation.GetGeolocationAccuracy)));
                            break;
                        }
                    case ServiceOperation.SetGeolocationAccuracy:
                        {
                            _manager.SetGeolocationAccuracy(Utils.GetData<int>(msg));
                            break;
                        }
                    case ServiceOperation.GetGeolocationListenerSettings:
                        {
                            _manager.GetGeolocationListenerSettings((listenerSettings) =>
                                Handle((messenger) => Utils.SendData(listenerSettings, messenger, _messenger, ServiceOperation.GetGeolocationListenerSettings)));
                            break;
                        }
                    case ServiceOperation.SetGeolocationListenerSettings:
                        {
                            _manager.SetGeolocationListenerSettings(Utils.GetData<GeolocationListenerSettings>(msg));
                            break;
                        }
                    case ServiceOperation.Close:
                        {
                            Stop();
                            break;
                        }
                }
            }
            catch (System.Exception e)
            {
                Log.Warn("Error in Handler_HasCome", e);
            }
        }

        private void InitWakeLock()
        {
            _wakelock = PowerManager.NewWakeLock(WakeLockFlags.Partial, "lazurite::servicewakelock");
            _wakelock.SetReferenceCounted(false);
        }

        private void ReleaseWakeLock()
        {
            _wakelock?.Release();
            _wakelock = null;
        }

        private void Stop()
        {
            OnDestroyInternal();
            StopTimer();
            StopForeground(true);
            StopSelf();
        }

        private void OnDestroyInternal()
        {
            SafeUnregisterReceiver(ScreenOnReciever);
            SafeUnregisterReceiver(StartLazuriteServiceReceiver);
            SafeUnregisterReceiver(GpsOnReciever);
            ReleaseWakeLock();
            _currentNotification?.Dispose();
            _currentNotification = null;
            _manager?.Close();
            _manager = null;

            AlreadyStarted = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public override void OnDestroy()
        {
            OnDestroyInternal();
            base.OnDestroy();
        }

        private enum TimerAction
        {
            Stop,
            Start,
            Nothing
        }
    }
}