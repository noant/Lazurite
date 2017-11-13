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
    public class IncomingHandler: Handler
    {
        public IncomingHandler()
        {
        }
        
        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            HasCome?.Invoke(this, msg);
        }

        public event Action<object, Message> HasCome;
    }
}