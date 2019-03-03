using Android.App;
using Android.Content;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.location.GPS_ENABLED_CHANGE" })]
    public class GpsOnReciever : BroadcastReceiver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public override void OnReceive(Context context, Intent intent)
        {
            if (Singleton.Any<IGeolocationListener>())
            {
                // Try to start listener
                Singleton
                    .Resolve<IGeolocationListener>()
                    .StartListenChanges();
            }
        }
    }
}