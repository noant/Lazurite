using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lazurite.MainDomain;

namespace LazuriteMobile.App.Droid
{
    public class SystemUtils : ISystemUtils
    {
        public void Sleep(int ms, CancellationToken cancelToken)
        {
            Thread.Sleep(ms);
        }
    }
}