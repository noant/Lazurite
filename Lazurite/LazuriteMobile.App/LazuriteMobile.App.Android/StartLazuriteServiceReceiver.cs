
using Android.App;
using Android.Content;

namespace LazuriteMobile.App.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class StartLazuriteServiceReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Application.Context.StartService(new Intent(context, typeof(LazuriteService)));
        }
    }
}