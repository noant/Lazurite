
using Android.App;
using Android.Content;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionScreenOn })]
    public class ScreenOnReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (Singleton.Any<LazuriteContext>())
            {
                var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
                scenariosManager.ScreenOnActions();
            }
        }
    }
}