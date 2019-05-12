using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using System;
using System.Linq;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    [Activity(Label = "Lazurite", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, HardwareAccelerated = true)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IHardwareVolumeChanger, ISupportsResume
    {
        SupportsResumeStateChanged ISupportsResume.StateChanged { get; set; }

        private SupportsResumeState _currentState = SupportsResumeState.Closed;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private void RaiseStateChanged(SupportsResumeState current, SupportsResumeState previous)
        {
            ((ISupportsResume)this).StateChanged?.Invoke(this, current, previous);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        protected override void OnCreate(Bundle bundle)
        {
            CrossCurrentActivity.Current.Activity = this;

            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Add(this);

            Singleton.Clear<ISupportsResume>();
            Singleton.Add(this);

            var context = Singleton.Resolve<LazuriteContext>();
            context.Manager = new ServiceConnectionManager(this);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            base.OnCreate(bundle);
            LoadApplication(new App());

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, global::Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (Singleton.Any<IRuntimePermissionsHandler>() && grantResults?.Length > 0)
            {
                var permissionsHandler = Singleton.Resolve<IRuntimePermissionsHandler>();
                permissionsHandler.ResolvePermissionCallback(requestCode, Enumerable.Range(0, permissions.Length).ToDictionary(x => permissions[x], x => grantResults[x] == Permission.Granted));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        protected override void OnDestroy()
        {
            OnCloseActions();
            base.OnDestroy();
        }

        private void OnCloseActions()
        {
            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Clear<ISupportsResume>();
            var manager = Singleton.Resolve<LazuriteContext>().Manager;
            manager.GetListenerSettings(s =>
            {
                if (s.TurnOffBackgroundWork)
                    manager.Close();
                else
                    manager.Unbind();
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        protected override void OnResume()
        {
            base.OnResume();
            RaiseStateChanged(SupportsResumeState.Active, _currentState);
            _currentState = SupportsResumeState.Active;
            HandleIntent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
            HandleIntent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private void HandleIntent()
        {
            try
            {
                if (Intent != null &&
                    Intent.Extras != null &&
                    Intent.Extras.ContainsKey(Keys.NeedOpenNotifications) &&
                    Intent.Extras.GetInt(Keys.NeedOpenNotifications) != -1)
                {
                    var handler = Singleton.Resolve<INotificationsHandler>();
                    handler.UpdateNotificationsInfo();
                    Intent = null;
                }
            }
            catch (Exception e)
            {
                Singleton.Resolve<ILogger>().Error(exception: e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event EventsHandler<int> VolumeUp;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public event EventsHandler<int> VolumeDown;
    }
}