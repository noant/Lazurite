using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LazuriteMobile.MainDomain;
using LazuriteMobile.Android.ServiceClient;
using Lazurite.IOC;
using Lazurite.Data;

namespace LazuriteMobile.App.Droid
{
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            if (!Singleton.Any<ISavior>())
                Singleton.Add(new JsonFileSavior());

            if (!Singleton.Any<IServiceClientManager>())
                Singleton.Add(new ServiceClientManager());

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

