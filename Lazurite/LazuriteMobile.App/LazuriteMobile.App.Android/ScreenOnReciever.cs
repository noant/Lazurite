
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public override void OnReceive(Context context, Intent intent)
        {
            if (Singleton.Any<LazuriteContext>())
            {
                var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
                scenariosManager?.ScreenOnActions(); // В старых версиях андройда это свойство может быть не инициализировано при разблокировке экрана
            }
        }
    }
}