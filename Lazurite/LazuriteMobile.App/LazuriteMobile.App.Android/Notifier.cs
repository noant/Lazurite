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
using Lazurite.Shared;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    public class Notifier : INotifier
    {
        private static int _lastId = 1;
        private static int GetNextNotificationId() => ++_lastId;
        private List<LazuriteNotification> _notificationsCache = new List<LazuriteNotification>();

        public LazuriteNotification[] GetNotifications()
        {
            try
            {
                return _notificationsCache.ToArray();
            }
            finally
            {
                foreach (var lazNotification in _notificationsCache)
                    lazNotification.IsRead = true;
            }
        }

        public void Notify(Lazurite.Shared.Message message)
        {
            var lazNotification = new LazuriteNotification();
            lazNotification.Message = message;
            _notificationsCache.Insert(0, lazNotification);

            var context = global::Android.App.Application.Context;

            var builder = new Notification.Builder(context);
            builder.SetContentTitle(message.Header);
            builder.SetContentText(message.Text);
            builder.SetSmallIcon(Resource.Drawable.icon);
            builder.SetVisibility(NotificationVisibility.Private);
            builder.SetColor(Color.Argb(0, 255, 255, 255).ToArgb());
            
            var activityIntent = new Intent(context, typeof(MainActivity));
            activityIntent.PutExtra(Keys.NeedOpenNotifications, new char[0]);

            var showActivityIntent = PendingIntent.GetActivity(Application.Context, 0,
                activityIntent, PendingIntentFlags.UpdateCurrent);

            builder.SetContentIntent(showActivityIntent);

            var notification = builder.Build();

            var notificationManager =
                context.GetSystemService(Context.NotificationService) as NotificationManager;
            
            notificationManager.Notify(GetNextNotificationId(), notification);
        }
    }
}