
using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;

namespace LazuriteMobile.App.Droid
{
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}