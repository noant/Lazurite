using Lazurite.IOC;
using LazuriteMobile.App.Controls;
using LazuriteMobile.MainDomain;
using System;
using System.Threading;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class MainPage : ContentPage
	{
        IScenariosManager _manager = Singleton.Resolve<LazuriteContext>().Manager;
        
        SynchronizationContext _currentContext = SynchronizationContext.Current;
        
        public MainPage()
		{
            this.InitializeComponent();

            _manager.ConnectionError += _manager_ConnectionError;
            _manager.NeedRefresh += _manager_NeedRefresh;
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
            _manager.NeedClientSettings += _manager_NeedClientSettings;
            _manager.LoginOrPasswordInvalid += _manager_LoginOrPasswordInvalid;
            _manager.CredentialsLoaded += _manager_CredentialsLoaded;
            _manager.SecretCodeInvalid += _manager_SecretCodeInvalid;
            _manager.ScenariosChanged += _manager_ScenariosChanged;
            settingsView.ConnectClicked += SettingsView_ConnectClicked;
            _manager.Initialize((initialized) =>
            {
                if (initialized)
                    _manager.IsConnected((connected) =>
                    {
                        if (connected)
                        {
                            Refresh();
                            Invoke(() => HideCaption());
                        }
                        else
                        {
                            ReConnectAndRefresh();
                        }
                    });
                else
                    Invoke(() => ShowCaption("Ошибка сервиса...", true, true));
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
            if (this.sliderMenu.MenuVisible)
            {
                this.sliderMenu.Hide();
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
                this.sliderMenu.Show();
                this.swgrid.IsEnabled = false;
                ShowCaption("Ошибка при расшифровке данных...\r\nВозможно, секретный ключ сервера введен неверно", true, true);
            });
        }

        private void _manager_LoginOrPasswordInvalid()
        {
            Invoke(() => {
                this.sliderMenu.Show();
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
            this.sliderMenu.Hide();
            var credentials = this.settingsView.GetCredentials();
            if (string.IsNullOrEmpty(credentials.Host) || string.IsNullOrEmpty(credentials.Login)
                || string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.SecretKey)
                || string.IsNullOrEmpty(credentials.ServiceName) || credentials.Port < 1)
            {
                ShowCaption("Не все поля введены", true);
                this.sliderMenu.Show();
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
                this.sliderMenu.Show();
                ShowCaption("Необходим ввод логина/пароля", false, false);
            });
        }

        private void _manager_ConnectionRestored()
        {
            Invoke(() => {
                this.sliderMenu.Hide();
                HideCaption();
                swgrid.IsEnabled = true;
            });
        }

        private void _manager_NeedRefresh()
        {
            Invoke(Refresh);
        }
        
        private void Refresh()
        {
            _manager.GetClientSettings((settings) => {
                Invoke(() => settingsView.SetCredentials(settings));
            });
            _manager.GetScenarios((scenarios) => {
                Invoke(() => swgrid.Refresh(scenarios));
            });
        }

        private void ReConnectAndRefresh()
        {
            _manager.ReConnect();
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
    }
}