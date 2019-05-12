using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteMobile.App.Common;
using LazuriteMobile.App.Controls;
using LazuriteMobile.MainDomain;
using System;
using System.Threading;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class MainPage : ContentPage, INotificationsHandler
    {
        private ILogger _log = Singleton.Resolve<ILogger>();
        private IClientManager _manager = Singleton.Resolve<LazuriteContext>().Manager;
        private ISupportsResume _supportsResume = Singleton.Resolve<ISupportsResume>();
        private SynchronizationContext _currentContext = SynchronizationContext.Current;
        private bool _initialized;

        private event EventsHandler<bool> ConnectionToServiceInitialized;

        public MainPage()
        {
            InitializeComponent();

            Singleton.Clear<INotificationsHandler>();
            Singleton.Add(this);

            tabsView.AddTabInfo(new SliderTabsView.TabInfo(connectionSettingsSlider, LazuriteUI.Icons.Icon.KeyOld));
            tabsView.AddTabInfo(new SliderTabsView.TabInfo(settingsSlider, LazuriteUI.Icons.Icon.Settings));
            tabsView.AddTabInfo(new SliderTabsView.TabInfo(messagesSlider, LazuriteUI.Icons.Icon.EmailMinimal));

            _supportsResume.StateChanged = (sender, currentState, previousState) =>
            {
                // Do not reinit when app was "home button pressed"
                if (currentState == SupportsResumeState.Paused)
                {
                    DialogView.CloseAllDialogs();
                }

                if (previousState == SupportsResumeState.Closed || previousState == SupportsResumeState.Stopped)
                {
                    InitializeManager();
                }
            };

            connectionView.ConnectClicked += ConnectionView_ConnectClicked;
            _manager.ConnectionError += _manager_ConnectionError;
            _manager.NeedRefresh += _manager_NeedRefresh;
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
            _manager.NeedClientSettings += _manager_NeedClientSettings;
            _manager.LoginOrPasswordInvalid += _manager_LoginOrPasswordInvalid;
            _manager.BruteforceSuspition += _manager_BruteforceSuspition;
            _manager.CredentialsLoaded += _manager_CredentialsLoaded;
            _manager.SecretCodeInvalid += _manager_SecretCodeInvalid;
            _manager.ScenariosChanged += _manager_ScenariosChanged;

            ShowCaption();
        }

        public async void InitializeManager()
        {
            await Helper.TryGrantRequiredPermissions();
            _manager.Initialize((initialized) =>
            {
                if (initialized)
                {
                    _manager.IsConnected((state) =>
                    {
                        try
                        {
                            Refresh(false);
                            if (state == ManagerConnectionState.Connected)
                            {
                                Invoke(HideCaption);
                            }
                            else if (state == ManagerConnectionState.Disconnected)
                            {
                                ReConnectAndRefresh();
                            }

                            _initialized = true;
                            ConnectionToServiceInitialized?.Invoke(this, new EventsArgs<bool>(_initialized));
                        }
                        catch (Exception e)
                        {
                            _log.Error(exception: e);
                        }
                    });
                }
                else
                {
                    Invoke(() => ShowCaption("Ошибка сервиса...", true, true, true));
                    _initialized = false;
                    ConnectionToServiceInitialized?.Invoke(this, new EventsArgs<bool>(_initialized));
                }
            });
        }

        private void _manager_ConnectionError()
        {
            Invoke(() =>
            {
                ShowCaption("Восстановление соединения...", true, false, true);
                swgrid.IsEnabled = false;
            });
        }

        private void _manager_ConnectionLost()
        {
            Invoke(() =>
            {
                ShowCaption("Восстановление соединения...", true, false, true);
                swgrid.IsEnabled = false;
            });
        }

        private void _manager_ScenariosChanged(Lazurite.MainDomain.ScenarioInfo[] changedScenarios)
        {
            Invoke(() => swgrid.RefreshLE(changedScenarios));
        }

        protected override bool OnBackButtonPressed()
        {
            if (DialogView.AnyOpened)
            {
                DialogView.CloseLast();
                return true;
            }
            else if (tabsView.AnyOpened())
            {
                tabsView.HideAll();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private void _manager_SecretCodeInvalid()
        {
            Invoke(() =>
            {
                connectionSettingsSlider.Show();
                swgrid.IsEnabled = false;
                ShowCaption("Ошибка при расшифровке данных...\r\nВозможно, секретный ключ сервера введен неверно", true, true, false);
            });
        }

        private void _manager_LoginOrPasswordInvalid()
        {
            Invoke(() =>
            {
                connectionSettingsSlider.Show();
                swgrid.IsEnabled = false;
                ShowCaption("Логин или пароль введен неверно", true, true, false);
            });
        }

        private void _manager_BruteforceSuspition()
        {
            Invoke(() =>
            {
                connectionSettingsSlider.Show();
                swgrid.IsEnabled = false;
                ShowCaption("Логин или пароль введен неверно. Сервер заблокировал доступ на 2 часа.", true, true, false);
            });
        }

        private void _manager_CredentialsLoaded()
        {
            _manager.GetClientSettings((settings) =>
            {
                Invoke(() => connectionView.SetCredentials(settings));
            });
        }

        private async void ConnectionView_ConnectClicked(ConnectionView obj)
        {
            await connectionSettingsSlider.Hide();
            var credentials = connectionView.GetCredentials();
            if (string.IsNullOrEmpty(credentials.Host) || string.IsNullOrEmpty(credentials.Login)
                || string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.SecretKey)
                || credentials.Port < 1)
            {
                ShowCaption("Не все поля введены", true, true, false);
                connectionSettingsSlider.Show();
            }
            else
            {
                ShowCaption($"Соединение с\r\n[{credentials.GetAddress()}]", true, false, true);
                _manager.SetClientSettings(credentials);
            }
        }

        private void _manager_NeedClientSettings()
        {
            Invoke(() =>
            {
                connectionSettingsSlider.Show();
                ShowCaption("Необходим ввод параметров подключения", true, true, false);
            });
        }

        private void _manager_ConnectionRestored()
        {
            Invoke(() =>
            {
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
                connectionSettingsSlider.Hide();
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до завершения вызова
                HideCaption();
                swgrid.IsEnabled = true;
            });
        }

        private void _manager_NeedRefresh()
        {
            Invoke(() => Refresh());
        }

        private void Refresh(Action callback, bool showMessageIfScenariosEmpty = true)
        {
            RefreshCredentials();
            _manager.GetScenarios((scenarios) =>
            {
                Invoke(() =>
                {
                    if (scenarios == null)
                        scenarios = new ScenarioInfo[0];

                    swgrid.Refresh(scenarios, showMessageIfScenariosEmpty);
                    callback?.Invoke();
                });
            });
        }

        public void Refresh(bool showMessageIfScenariosEmpty = true) => Refresh(null, showMessageIfScenariosEmpty);

        private void ReConnectAndRefresh()
        {
            _manager.ReConnect();
            RefreshCredentials();
        }

        private void RefreshCredentials()
        {
            _manager.GetClientSettings((settings) =>
            {
                Invoke(() => connectionView.SetCredentials(settings));
            });
        }

        public void HideCaption()
        {
            gridCaption.IsVisible = false;
            iconAnimation.StopAnimate();
            lblCaption.Text = string.Empty;
            connectionView.SetErrorMessage(string.Empty);
        }

        public void ShowCaption(string text = "", bool showLoadingGrid = true, bool showTextOnLoadingGrid = true, bool showLoadingAnimation = true)
        {
            // Close dialogviews
            DialogView.CloseAllDialogs(all: false);

            if (showLoadingGrid)
            {
                gridCaption.IsVisible = true;
                if (showLoadingAnimation)
                {
                    iconAnimation.StartAnimate();
                }
                else
                {
                    iconAnimation.StopAnimate();
                }
            }

            lblCaption.Text = showTextOnLoadingGrid ? text : string.Empty;

            connectionView.SetErrorMessage(text);
        }

        private void Invoke(Action action)
        {
            _currentContext.Post(new SendOrPostCallback((s) => action?.Invoke()), null);
        }

        void INotificationsHandler.UpdateNotificationsInfo()
        {
            if (_initialized)
            {
                Invoke(ShowNotifications);
            }
            else
            {
                ConnectionToServiceInitialized += (o, e) => Invoke(ShowNotifications);
            }
        }

        bool INotificationsHandler.NeedViewPermanently => messagesSlider.MenuVisible;

        private void ShowNotifications()
        {
            if (messagesSlider.MenuVisible)
            {
                messagesView.UpdateMessages();
            }
            else
            {
                messagesSlider.Show();
            }
        }
    }
}