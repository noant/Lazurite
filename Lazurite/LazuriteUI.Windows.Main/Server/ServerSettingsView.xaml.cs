using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Shared;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Common;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Server
{
    /// <summary>
    /// Логика взаимодействия для ServerSettingsView.xaml
    /// </summary>
    [DisplayName("Настройки сервера")]
    [LazuriteIcon(Icon.Server)]
    public sealed partial class ServerSettingsView : UserControl, IDisposable
    {
        private readonly WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private readonly LazuriteServer _server = Singleton.Resolve<LazuriteServer>();
        private readonly DataEncryptorBase _dataEncryptor = Singleton.Resolve<DataEncryptorBase>();
        private readonly DataManagerBase _dataManager = Singleton.Resolve<DataManagerBase>();
        private ServerSettings _settings;

        public ServerSettingsView()
        {
            InitializeComponent();

            var strOriginalBtChangeFileSecretKeyText = btChangeFileSecretKey.Content.ToString();
            if (!_dataEncryptor.IsSecretKeyExist)
                btChangeFileSecretKey.Content = "Задать ключ шифрования файлов";

            _settings = (ServerSettings)Lazurite.Windows.Utils.Utils.CloneObject(_server.Settings);
            tbPort.Validation = (v) => EntryViewValidation.UShortValidation().Invoke(v);
            tbPort.TextChanged += (o, e) => SettingsChanged();
            btChangeCert.Click += (o, e) => CertificateSelectView.Show(_settings, (s) => SettingsChanged());
            _server.StatusChanged += Server_StatusChanged;
            btChangeSecretKey.Click += (o, e) => {
                EnterPasswordView.Show(
                    "Введите новый секретный ключ сервера...",
                    (pass) => {
                        _settings.SecretKey = pass;
                        SettingsChanged();
                    },
                    (pass) => pass.Length == 16,
                    "Длина секретного ключа должна быть равна 16-и символам");
            };
            btChangeFileSecretKey.Click += (o, e) => {
                EnterPasswordView.Show(
                    "Введите ключ шифрования файлов...",
                    (pass) => {
                        _dataEncryptor.SecretKey = pass;
                        btChangeFileSecretKey.Content = strOriginalBtChangeFileSecretKeyText;
                    },
                    (pass) => pass.Length == 16,
                    "Длина секретного ключа должна быть равна 16-и символам");
            };

            btApply.Click += (o, e) => {
                try
                {
                    _settings.Port = ushort.Parse(tbPort.Text);
                    _server.Settings = _settings;
                    btApply.IsEnabled = false;
                    _server.Restart(null);
                }
                catch (Exception exception)
                {
                    _warningHandler.Error("Во время применения настроек сервера произошла ошибка.", exception);
                }
            };

            Unloaded += (o, e) => Dispose();

            Refresh();
        }

        private void Server_StatusChanged(object sender, EventsArgs<LazuriteServer> args)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateServerInfo()));
        }

        public void UpdateServerInfo()
        {
            lblCurrentAddress.Content = _server.Settings.GetAddress();
            lblCurrentStatus.Content = _server.Started ? "Запущен" : "!Отключен";
        }

        public void SettingsChanged()
        {
            btApply.IsEnabled = true;
        }

        public void Refresh()
        {
            UpdateServerInfo();
            tbPort.Text = _settings.Port.ToString();
            btApply.IsEnabled = false;
        }

        public void Dispose()
        {
            _server.StatusChanged -= Server_StatusChanged;
        }
    }
}
