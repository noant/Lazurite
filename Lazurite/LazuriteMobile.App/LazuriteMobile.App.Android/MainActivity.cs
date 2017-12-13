
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Lazurite.IOC;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Droid
{
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, HardwareAccelerated = true)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IHardwareVolumeChanger, ISupportsResume
    {
        Action<ISupportsResume> ISupportsResume.OnResume
        {
            get;
            set;
        }

        protected override void OnCreate(Bundle bundle)
        {
            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Add((IHardwareVolumeChanger)this);

            Singleton.Clear<ISupportsResume>();
            Singleton.Add((ISupportsResume)this);

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
            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Clear<ISupportsResume>();
            Singleton.Resolve<LazuriteContext>().Manager.Close();
            base.OnDestroy();
        }
        
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeDown)
            {
                VolumeDown?.Invoke(this, new EventsArgs<int>(-1));
                return true;
            }
            else if (keyCode == Keycode.VolumeUp)
            {
                VolumeUp?.Invoke(this, new EventsArgs<int>(1));
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnResume()
        {
            base.OnResume();
            ((ISupportsResume)this).OnResume?.Invoke(this);
        }

        public event EventsHandler<int> VolumeUp;
        public event EventsHandler<int> VolumeDown;
    }
}

