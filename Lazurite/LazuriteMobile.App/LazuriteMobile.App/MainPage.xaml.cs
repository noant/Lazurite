using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
	public partial class MainPage : ContentPage
	{
        IScenariosManager _manager = Singleton.Resolve<IScenariosManager>();
        
        SynchronizationContext _currentContext = SynchronizationContext.Current;

        public MainPage()
		{
            this.InitializeComponent();
            _manager.NeedRefresh += _manager_NeedRefresh;
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
            _manager.NeedClientSettings += _manager_NeedClientSettings;
            _manager.LoginOrPasswordInvalid += _manager_LoginOrPasswordInvalid;
            _manager.CredentialsLoaded += _manager_CredentialsLoaded;
            _manager.SecretCodeInvalid += _manager_SecretCodeInvalid;
            settingsView.ConnectClicked += SettingsView_ConnectClicked;
        }
                
        private void _manager_SecretCodeInvalid()
        {
            _currentContext.Post((t) => {
                this.sliderMenu.Show();
                ShowCaption("Ошибка при расшифровке данных...\r\nВозможно, секретный ключ сервера введен неверно", true, false);
            }, null);
        }

        private void _manager_LoginOrPasswordInvalid()
        {
            _currentContext.Post((t) => {
                this.sliderMenu.Show();
                ShowCaption("Логин или пароль введен неверно", true, false);
            }, null);
        }

        private void _manager_CredentialsLoaded()
        {
            settingsView.SetCredentials(_manager.GetClientSettings());
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
            _currentContext.Post((t) => {
                this.sliderMenu.Show();
                ShowCaption("Необходим ввод логина/пароля", false, false);
            }, null);
        }

        private void _manager_ConnectionRestored()
        {
            _currentContext.Post((state) => {
                HideCaption();
            }, null);
        }

        private void _manager_ConnectionLost()
        {
            _currentContext.Post((state) => {
                ShowCaption("Соединение разорвано...", true);
            }, null);
        }

        private void _manager_NeedRefresh()
        {
            _currentContext.Post((state) => {
                settingsView.SetCredentials(_manager.GetClientSettings());
                HideCaption();
                swgrid.Refresh(_manager.Scenarios);
            }, null);
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
    }
}