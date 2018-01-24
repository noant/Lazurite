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
using Lazurite.Shared;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    public class Notifier : INotifier
    {
        public void Notify(Lazurite.Shared.Message message)
        {
            throw new NotImplementedException();
        }
    }
}