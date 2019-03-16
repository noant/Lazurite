using Android.App;
using Android.Content;
using Android.OS;

namespace LazuriteMobile.App.Droid
{
    public class BackgroundReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                Application.Context.StartForegroundService(new Intent(Application.Context, typeof(LazuriteService)));
            else
                Application.Context.StartService(new Intent(Application.Context, typeof(LazuriteService)));
        }
    }
}