using Lazurite.IOC;
using Lazurite.Shared;
using LazuriteMobile.App.Common;
using LazuriteMobile.App.Controls;
using LazuriteMobile.MainDomain;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : Grid, IUpdatable
    {
        public SettingsView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private int _currentGelocationAccuracy;
        private GeolocationListenerSettings _currentGeolocationSettings;
        private ListenerSettings _currentListenerSettings;

        public void SelectSkin(object sender, EventsArgs<SettingsView> sv)
        {
            SkinSelectView.Show(DialogView.GetDialogHost(this));
        }

        public async void SelectSetting_Listener(SettingsItem settingItem)
        {
            await Helper.TryGrantRequiredPermissions();

            _currentListenerSettings = (ListenerSettings)settingItem.Tag;
            var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
            scenariosManager.SetListenerSettings(_currentListenerSettings);
            scenariosManager.ReInitialize();
        }

        public void IsSelected_Listener(SettingsItem.IsSelectedResult args)
        {
            var mode = (ListenerSettings)args.SettingsItem.Tag;
            args.IsSelected = _currentListenerSettings.Equals(mode);
        }

        public async void SelectSetting_GeolocationListener(SettingsItem settingItem)
        {
            await Helper.TryGrantRequiredPermissions();

            _currentGeolocationSettings = (GeolocationListenerSettings)settingItem.Tag;
            var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
            scenariosManager.SetGeolocationListenerSettings(_currentGeolocationSettings);
        }

        public void IsSelected_GeolocationListener(SettingsItem.IsSelectedResult args)
        {
            var mode = (GeolocationListenerSettings)args.SettingsItem.Tag;
            args.IsSelected = _currentGeolocationSettings.Equals(mode);
        }

        public async void SelectSetting_GeolocationAccuracy(SettingsItem settingItem)
        {
            await Helper.TryGrantRequiredPermissions();

            _currentGelocationAccuracy = int.Parse(settingItem.Tag.ToString());
            var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
            scenariosManager.SetGeolocationAccuracy(_currentGelocationAccuracy);
        }

        public void IsSelected_GeolocationAccuracy(SettingsItem.IsSelectedResult args)
        {
            var accuracy = int.Parse(args.SettingsItem.Tag.ToString());
            args.IsSelected = _currentGelocationAccuracy == accuracy;
        }

        public ICommand XiaomiAndEtcUsersClick => new Command<string>((url) =>
        {
            Device.OpenUri(new System.Uri(url));
        });

        public void UpdateView(Action callback)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Singleton.Any<LazuriteContext>())
                {
                    var i = 0;
                    var locker = new object();
                    void raiseCallBackIfAllComplete() // Crutch
                    {
                        lock (locker)
                        {
                            i++;
                            if (i == 3)
                            {
                                callback?.Invoke();
                            }
                        }
                    }

                    warnMessageView.IsVisible = false;
                    var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
                    scenariosManager.GetListenerSettings((settings) =>
                    {
                        _currentListenerSettings = settings;
                        raiseCallBackIfAllComplete();
                    });
                    scenariosManager.GetGeolocationListenerSettings((gpsSettings) =>
                    {
                        _currentGeolocationSettings = gpsSettings;
                        raiseCallBackIfAllComplete();
                    });
                    scenariosManager.GetGeolocationAccuracy((gpsAccuracy) =>
                    {
                        _currentGelocationAccuracy = gpsAccuracy;
                        raiseCallBackIfAllComplete();
                    });
                }
                else
                {
                    warnMessageView.IsVisible = true;
                    callback?.Invoke();
                }
            });
        }

        private void HelpItemView_Click(object sender, EventsArgs<object> args)
        {
            Device.OpenUri(new Uri("https://github.com/noant/Lazurite/wiki/%D0%94%D0%BE%D0%BF%D0%BE%D0%BB%D0%BD%D0%B8%D1%82%D0%B5%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F-%D0%B8%D0%BD%D1%84%D0%BE%D1%80%D0%BC%D0%B0%D1%86%D0%B8%D1%8F-%D0%BE-%D0%BA%D0%BB%D0%B8%D0%B5%D0%BD%D1%82%D0%B5"));
        }
    }
}