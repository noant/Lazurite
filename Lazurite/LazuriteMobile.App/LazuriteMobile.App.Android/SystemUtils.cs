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
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(300);

        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(CancellationToken.None))
                Thread.Sleep(ms);
            else for (int i = 0; i <= ms && !cancelToken.CanBeCanceled; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }
    }
}