
using Android.App;
using Android.Content;

namespace LazuriteMobile.App.Droid
{
    public class BackgroundReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (!LazuriteService.Started)
                Application.Context.StartService(new Intent(Application.Context, typeof(LazuriteService)));
        }
    }
}