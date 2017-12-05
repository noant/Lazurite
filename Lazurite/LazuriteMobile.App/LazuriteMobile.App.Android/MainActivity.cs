
using Android.App;
using Android.Content.PM;
using Android.OS;
using Lazurite.IOC;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, HardwareAccelerated = true)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            if (!Singleton.Any<LazuriteContext>())
                Singleton.Add(new LazuriteContext());

            var context = Singleton.Resolve<LazuriteContext>();
            context.Manager = new ServiceScenariosManager(this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            Singleton.Resolve<LazuriteContext>().Manager.Close();
            base.OnDestroy();
        }
    }
}

