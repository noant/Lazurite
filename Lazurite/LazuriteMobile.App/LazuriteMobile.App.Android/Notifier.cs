using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lazurite.IOC;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;

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
                    if (!notification.IsRead)
                        notification.IsRead = _read.Contains(notification);
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
                    notificationManager.Cancel(notification.Id);

                if (_notificationsCache.Count > MaxMessagesCnt)
                    _notificationsCache = _notificationsCache.GetRange(0, MaxMessagesCnt);
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

            var notificationManager =
                context.GetSystemService(Context.NotificationService) as NotificationManager;

            Notification.Builder builder;

            if (Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
            {
                var channelId = "channel1";
                NotificationChannel mChannel;

                mChannel = new NotificationChannel(channelId, "laz", NotificationImportance.High);
                mChannel.EnableLights(true);
                mChannel.LightColor = Color.White;
                mChannel.SetShowBadge(true);
                mChannel.LockscreenVisibility = NotificationVisibility.Private;
                notificationManager.CreateNotificationChannel(mChannel);
                builder = new Notification.Builder(context, channelId);
            }
            else
#pragma warning disable CS0618 // Тип или член устарел
                builder = new Notification.Builder(context);
#pragma warning restore CS0618 // Тип или член устарел

            builder.SetContentTitle(message.Header);
            builder.SetContentText(message.Text);
            builder.SetSmallIcon(Resource.Drawable.icon);
            builder.SetVisibility(NotificationVisibility.Private);
            builder.SetOnlyAlertOnce(true);
#pragma warning disable CS0618 // Тип или член устарел
            builder.SetDefaults(NotificationDefaults.All);
#pragma warning restore CS0618 // Тип или член устарел
            builder.SetAutoCancel(true);
            builder.SetColor(Color.Argb(0, 255, 255, 255).ToArgb());

            var activityIntent = new Intent(context, typeof(MainActivity));
            activityIntent.PutExtra(Keys.NeedOpenNotifications, newId);

            var showActivityIntent = PendingIntent.GetActivity(Application.Context, 0,
                activityIntent, PendingIntentFlags.UpdateCurrent);

            builder.SetContentIntent(showActivityIntent);

            var notification = builder.Build();

            notificationManager.Notify(newId, notification);

            if (Singleton.Any<INotificationsHandler>())
            {
                var notificationHandler = Singleton.Resolve<INotificationsHandler>();
                if (notificationHandler.NeedViewPermanently)
                    notificationHandler.UpdateNotificationsInfo();
            }
        }
    }
}