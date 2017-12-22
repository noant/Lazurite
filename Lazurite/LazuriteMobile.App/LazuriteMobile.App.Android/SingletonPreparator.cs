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
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using LazuriteMobile.Android.ServiceClient;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    public static class SingletonPreparator
    {
        public static void Initialize()
        {
            if (!Singleton.Any<ILogger>())
                Singleton.Add(new LogStub());
            if (!Singleton.Any<LazuriteContext>())
                Singleton.Add(new LazuriteContext());
            if (!Singleton.Any<SaviorBase>())
                Singleton.Add(new JsonFileSavior());
            if (!Singleton.Any<ISystemUtils>())
                Singleton.Add(new SystemUtils());
            if (!Singleton.Any<IServiceClientManager>())
                Singleton.Add(new ServiceClientManager());
        }
    }
}