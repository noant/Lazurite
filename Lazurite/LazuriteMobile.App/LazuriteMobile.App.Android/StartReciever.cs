using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazuriteMobile.App.Droid
{
    public class StartReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var serviceIntent = new Intent(context, typeof(LazuriteService));
            Application.Context.StartService(serviceIntent);
        }
    }
}