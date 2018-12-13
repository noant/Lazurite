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
            InitializeComponent();

            Singleton.Clear<INotificationsHandler>();
            Singleton.Add((INotificationsHandler)this);

            iconAnimation.StartAnimate();

            tabsView.AddTabInfo(new SliderTabsView.TabInfo(connectionSettingsSlider, LazuriteUI.Icons.Icon.Settings));
            tabsView.AddTabInfo(new SliderTabsView.TabInfo(messagesSlider, LazuriteUI.Icons.Icon.EmailMinimal));
            _supportsResume.StateChanged = (sender, currentState, previousState) =>
            {
                //do not reinit when app was "home button pressed"
                if (currentState == SupportsResumeState.Paused)
                    DialogView.CloseAllDialogs();
                if (previousState == SupportsResumeState.Closed || previousState == SupportsResumeState.Stopped)
                    InitializeManager();
            };
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
                            Refresh();
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
            if (tabsView.AnyOpened())
            {
                tabsView.HideAll();
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
                connectionSettingsSlider.Show();
                swgrid.IsEnabled = false;
                ShowCaption("Ошибка при расшифровке данных...\r\nВозможно, секретный ключ сервера введен неверно", true, true);
            });
        }

        private void _manager_LoginOrPasswordInvalid()
        {
            Invoke(() => {
                connectionSettingsSlider.Show();
                swgrid.IsEnabled = false;
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
            connectionSettingsSlider.Hide();
            var credentials = settingsView.GetCredentials();
            if (string.IsNullOrEmpty(credentials.Host) || string.IsNullOrEmpty(credentials.Login)
                || string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.SecretKey)
                || string.IsNullOrEmpty(credentials.ServiceName) || credentials.Port < 1)
            {
                ShowCaption("Не все поля введены", true);
                connectionSettingsSlider.Show();
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
                connectionSettingsSlider.Show();
                ShowCaption("Необходим ввод логина/пароля", true, false);
            });
        }

        private void _manager_ConnectionRestored()
        {
            Invoke(() => {
                connectionSettingsSlider.Hide();
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
            gridCaption.IsVisible = false;
            iconAnimation.StopAnimate();
            lblCaption.Text = string.Empty;
            settingsView.SetErrorMessage(string.Empty);
        }

        public void ShowCaption(string text = "", bool error = false, bool showLoadingGrid = true)
        {
            //close dialogviews
            DialogView.CloseLast();

            if (showLoadingGrid)
            {
                gridCaption.IsVisible = true;
                if (!error)
                    iconAnimation.StartAnimate();
                else
                    iconAnimation.StopAnimate();
            }

            if (error)
                lblCaption.Text = text;

            settingsView.SetErrorMessage(text);
        }

        private void Invoke(Action action)
        {
            _currentContext.Post(new SendOrPostCallback((s) => action?.Invoke()), null);
        }

        void INotificationsHandler.UpdateNotificationsInfo()
        {
            if (_initialized)
                Invoke(ShowNotifications);
            else
                ConnectionToServiceInitialized += (o,e) => Invoke(ShowNotifications);
        }
        
        bool INotificationsHandler.NeedViewPermanently => messagesSlider.MenuVisible;

        private void ShowNotifications() {
            if (messagesSlider.MenuVisible)
                messagesView.UpdateMessages();
            else
                messagesSlider.Show();
        }
    }
}