
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Lazurite.IOC;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteMobile.MainDomain;
using System;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Content;

namespace LazuriteMobile.App.Droid
{
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, HardwareAccelerated = true)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IHardwareVolumeChanger, ISupportsResume
    {
        SupportsResumeStateChanged ISupportsResume.StateChanged { get; set; }

        private SupportsResumeState _currentState = SupportsResumeState.Closed;

        private void RaiseStateChanged(SupportsResumeState current, SupportsResumeState previous)
        {
            ((ISupportsResume)this).StateChanged?.Invoke(this, current, previous);
        }

        protected override void OnCreate(Bundle bundle)
        {
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Add((IHardwareVolumeChanger)this);

            Singleton.Clear<ISupportsResume>();
            Singleton.Add((ISupportsResume)this);
            
            var context = Singleton.Resolve<LazuriteContext>();
            context.Manager = new ServiceScenariosManager(this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, global::Android.Content.PM.Permission[] grantResults)
        {
            try
            {
                PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch
            {
                //crutch
            }
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
            RaiseStateChanged(SupportsResumeState.Active, _currentState);
            _currentState = SupportsResumeState.Active;
            HandleIntent(Intent);
        }

        protected override void OnPause()
        {
            base.OnPause();
            var newState = IsFinishing ? SupportsResumeState.Stopped : SupportsResumeState.Paused;
            RaiseStateChanged(newState, _currentState);
            _currentState = newState;
        }
                
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Intent = intent;
            HandleIntent(intent);
        }
        
        private Intent _handledIntent;


        private void HandleIntent(Intent intent)
        {
            if (intent != _handledIntent && 
                intent.Extras != null && 
                intent.Extras.ContainsKey(Keys.NeedOpenNotifications))
            {
                _handledIntent = intent;
                var handler = Singleton.Resolve<INotificationsHandler>();
                handler.UpdateNotificationsInfo();
            }
        }
        
        public event EventsHandler<int> VolumeUp;
        public event EventsHandler<int> VolumeDown;
    }
}

