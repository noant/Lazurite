using Lazurite.IOC;
using Lazurite.Shared;
using LazuriteMobile.App.Common;
using LazuriteMobile.App.Controls;
using LazuriteMobile.MainDomain;
using System;
using System.Linq;
using System.Threading;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class MainPage : ContentPage, INotificationsHandler
	{
        private IScenariosManager _manager = Singleton.Resolve<LazuriteContext>().Manager;
        private ISupportsResume _supportsResume = Singleton.Resolve<ISupportsResume>();        
        private SynchronizationContext _currentContext = SynchronizationContext.Current;
        private bool _initialized;
        private event EventsHandler<bool> ConnectionToServiceInitialized;

        public MainPage()
		{
            this.InitializeComponent();

            Singleton.Clear<INotificationsHandler>();
            Singleton.Add((INotificationsHandler)this);

            this.tabsView.AddTabInfo(new SliderTabsView.TabInfo(connectionSettingsSlider, LazuriteUI.Icons.Icon.Settings));
            this.tabsView.AddTabInfo(new SliderTabsView.TabInfo(messagesSlider, LazuriteUI.Icons.Icon.EmailMinimal));
            _supportsResume.OnResume = (s) => InitializeManager();
            settingsView.ConnectClicked += SettingsView_ConnectClicked;
            _manager.ConnectionError += _manager_ConnectionError;
            _manager.NeedRefresh += _manager_NeedRefresh;
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
            _manager.NeedClientSettings += _manager_NeedClientSettings;
            _manager.LoginOrPasswordInvalid += _manager_LoginOrPasswordInvalid;
            _manager.CredentialsLoaded += _manager_CredentialsLoaded;
            _manager.SecretCodeInvalid += _manager_SecretCodeInvalid;
            _manager.ScenariosChanged += _manager_ScenariosChanged;
        }

        private void InitializeManager()
        {
            _manager.Initialize((initialized) =>
            {
                if (initialized)
                {
                    _manager.IsConnected((state) =>
                    {
                        if (state == ManagerConnectionState.Connected)
                        {
                            //refhresh on user open
                            //Refresh(() => _manager.RefreshIteration());
                            Invoke(() => HideCaption());
                        }
                        else if (state == ManagerConnectionState.Disconnected)
                        {
                            ReConnectAndRefresh();
                        }
                        else
                        {
                            RefreshCredentials();
                        }
                        _initialized = true;
                        ConnectionToServiceInitialized?.Invoke(this, new EventsArgs<bool>(_initialized));
                    });
                }
                else
                {
                    Invoke(() => ShowCaption("Ошибка сервиса...", true, true));
                    _initialized = false;
                    ConnectionToServiceInitialized?.Invoke(this, new EventsArgs<bool>(_initialized));
                }
            });
        }

        private void _manager_ConnectionError()
        {
            Invoke(() => {
                ShowCaption("Восстановление соединения...");
                swgrid.IsEnabled = false;
            });
        }

        private void _manager_ConnectionLost()
        {
            Invoke(() => {
                ShowCaption("Восстановление соединения...");
                swgrid.IsEnabled = false;
            });
        }

        private void _manager_ScenariosChanged(Lazurite.MainDomain.ScenarioInfo[] changedScenarios)
        {
            Invoke(() => swgrid.RefreshLE(changedScenarios));
        }

        protected override bool OnBackButtonPressed()
        {
            if (this.tabsView.AnyOpened())
            {
                this.tabsView.HideAll();
                return true;
            }
            else if (DialogView.AnyOpened)
            {
                DialogView.CloseLast();
                return true;
            }
            else
                return base.OnBackButtonPressed();
        }
        
        private void _manager_SecretCodeInvalid()
        {
            Invoke(() => {
                this.connectionSettingsSlider.Show();
                this.swgrid.IsEnabled = false;
                ShowCaption("Ошибка при расшифровке данных...\r\nВозможно, секретный ключ сервера введен неверно", true, true);
            });
        }

        private void _manager_LoginOrPasswordInvalid()
        {
            Invoke(() => {
                this.connectionSettingsSlider.Show();
                this.swgrid.IsEnabled = false;
                ShowCaption("Логин или пароль введен неверно", true, true);
            });
        }

        private void _manager_CredentialsLoaded()
        {
            _manager.GetClientSettings((settings) => {
                Invoke(() => settingsView.SetCredentials(settings));
            });
        }

        private void SettingsView_ConnectClicked(SettingsView obj)
        {
            this.connectionSettingsSlider.Hide();
            var credentials = this.settingsView.GetCredentials();
            if (string.IsNullOrEmpty(credentials.Host) || string.IsNullOrEmpty(credentials.Login)
                || string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.SecretKey)
                || string.IsNullOrEmpty(credentials.ServiceName) || credentials.Port < 1)
            {
                ShowCaption("Не все поля введены", true);
                this.connectionSettingsSlider.Show();
            }
            else
            {
                ShowCaption("Соединение с\r\n[" + credentials.GetAddress() + "]");
                _manager.SetClientSettings(credentials);
            }
        }

        private void _manager_NeedClientSettings()
        {
            Invoke(() => {
                this.connectionSettingsSlider.Show();
                ShowCaption("Необходим ввод логина/пароля", false, false);
            });
        }

        private void _manager_ConnectionRestored()
        {
            Invoke(() => {
                this.connectionSettingsSlider.Hide();
                HideCaption();
                swgrid.IsEnabled = true;
            });
        }

        private void _manager_NeedRefresh()
        {
            Invoke(Refresh);
        }
        
        private void Refresh(Action callback)
        {
            RefreshCredentials();
            _manager.GetScenarios((scenarios) => {
                Invoke(() =>
                {
                    swgrid.Refresh(scenarios);
                    callback?.Invoke();
                });
            });
        }

        public void Refresh() => Refresh(null);

        private void ReConnectAndRefresh()
        {
            _manager.ReConnect();
            RefreshCredentials();
        }

        private void RefreshCredentials()
        {
            _manager.GetClientSettings((settings) =>
            {
                Invoke(() => settingsView.SetCredentials(settings));
            });
        }

        public void HideCaption()
        {
            this.gridCaption.IsVisible = false;
            lblCaption.Text = string.Empty;
            this.settingsView.SetErrorMessage(string.Empty);
        }

        public void ShowCaption(string text, bool error = false, bool show = true)
        {
            //close dialogviews
            DialogView.CloseLast();

            if (show)
                this.gridCaption.IsVisible = true;
            lblCaption.Text = text;
            lblCaption.TextColor = error ? Color.Purple : Color.White;
            this.settingsView.SetErrorMessage(text);
        }

        private void Invoke(Action action)
        {
            _currentContext.Post(new SendOrPostCallback((s) => action?.Invoke()), null);
        }

        void INotificationsHandler.UpdateNotificationsInfo()
        {
            if (_initialized)
                ShowNotifications();
            else
                this.ConnectionToServiceInitialized += (o, e) => ShowNotifications();
        }

        private void ShowNotifications() {
            if (messagesSlider.MenuVisible)
                messagesView.UpdateMessages();
            else
                messagesSlider.Show();
        }
    }
}