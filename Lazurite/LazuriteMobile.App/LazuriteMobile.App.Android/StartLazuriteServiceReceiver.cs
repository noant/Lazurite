using Android.App;
using Android.Content;
using Android.OS;

namespace LazuriteMobile.App.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class StartLazuriteServiceReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                Application.Context.StartForegroundService(new Intent(context, typeof(LazuriteService)));
            else
                Application.Context.StartService(new Intent(context, typeof(LazuriteService)));
        }
    }
}