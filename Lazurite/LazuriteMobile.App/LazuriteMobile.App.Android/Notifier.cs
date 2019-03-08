using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System.Collections.Generic;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    public class Notifier : INotifier
    {
        private const int MaxMessagesCnt = 20;
        private static int _lastId = 1;

        private static int GetNextNotificationId() => ++_lastId;

        private List<LazuriteNotification> _notificationsCache = new List<LazuriteNotification>();
        private List<LazuriteNotification> _read = new List<LazuriteNotification>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public LazuriteNotification[] GetNotifications()
        {
            try
            {
                foreach (var notification in _notificationsCache)
                {
                    if (!notification.IsRead)
                    {
                        notification.IsRead = _read.Contains(notification);
                    }
                }

                return _notificationsCache.ToArray();
            }
            finally
            {
                _read.Clear();
                _read.AddRange(_notificationsCache);

                var context = global::Android.App.Application.Context;

                var notificationManager =
                    context.GetSystemService(Context.NotificationService) as NotificationManager;

                foreach (var notification in _notificationsCache)
                {
                    notificationManager.Cancel(notification.Id);
                }

                if (_notificationsCache.Count > MaxMessagesCnt)
                {
                    _notificationsCache = _notificationsCache.GetRange(0, MaxMessagesCnt);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Notify(Lazurite.Shared.Message message)
        {
            var newId = GetNextNotificationId();

            var lazNotification = new LazuriteNotification();
            lazNotification.Id = newId;
            lazNotification.Message = message;
            _notificationsCache.Insert(0, lazNotification);

            var context = global::Android.App.Application.Context;

            var activityIntent = new Intent(context, typeof(MainActivity));
            activityIntent.PutExtra(Keys.NeedOpenNotifications, newId);

            var showActivityIntent = PendingIntent.GetActivity(Application.Context, 0, activityIntent, PendingIntentFlags.UpdateCurrent);

            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            var channelId = Build.VERSION.SdkInt >= BuildVersionCodes.O ? CreateNotificationChannel(notificationManager) : string.Empty;

            var notificationBuilder = new NotificationCompat.Builder(context, channelId);
            notificationBuilder.SetContentTitle(message.Header);
            notificationBuilder.SetContentText(message.Text);
            notificationBuilder.SetContentIntent(showActivityIntent);
            notificationBuilder.SetSmallIcon(Resource.Drawable.message);
            notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon));
            notificationBuilder.SetPriority((int)NotificationPriority.Max);
            notificationBuilder.SetDefaults((int)NotificationDefaults.All);
            notificationBuilder.SetColor(Color.Purple);
            notificationBuilder.SetOnlyAlertOnce(true);
            notificationBuilder.SetAutoCancel(true);
            notificationBuilder.SetCategory(Notification.CategoryMessage);
            notificationBuilder.SetVisibility((int)NotificationVisibility.Private);

            notificationManager.Notify(newId, notificationBuilder.Build());

            if (Singleton.Any<INotificationsHandler>())
            {
                var notificationHandler = Singleton.Resolve<INotificationsHandler>();
                if (notificationHandler.NeedViewPermanently)
                {
                    notificationHandler.UpdateNotificationsInfo();
                }
            }
        }

        private string CreateNotificationChannel(NotificationManager manager)
        {
            var channel = new NotificationChannel("lazurite_message_notification", "lazurite_message_notification", NotificationImportance.High);
            channel.LightColor = Color.Blue;
            channel.LockscreenVisibility = NotificationVisibility.Private;
            channel.Description = "Lazurite messages notifications";
            manager.CreateNotificationChannel(channel);
            return channel.Id;
        }
    }
}