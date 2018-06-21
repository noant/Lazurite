
using Android.App;
using Android.Content;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.location.GPS_ENABLED_CHANGE" })]
    public class GpsOnReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (Singleton.Any<IGeolocationListener>())
            {
                var listener = Singleton.Resolve<IGeolocationListener>();
                if (intent.GetBooleanExtra("enabled", false))
                    listener.StartListenChanges();
                else
                    listener.Stop();
            }
        }
    }
}