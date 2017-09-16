using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Server
{
    /// <summary>
    /// Логика взаимодействия для ServerSettingsView.xaml
    /// </summary>
    [DisplayName("Настройки сервера")]
    [LazuriteIcon(Icon.Server)]
    public partial class ServerSettingsView : UserControl, IDisposable
    {
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private LazuriteServer _server = Singleton.Resolve<LazuriteServer>();
        private ServerSettings _settings;

        public ServerSettingsView()
        {
            InitializeComponent();
            _settings = (ServerSettings)Lazurite.Windows.Utils.Utils.CloneObject(_server.GetSettings());
            tbPort.Validation = (o,v) => EntryViewValidation.UShortValidation().Invoke(o, v);
            tbServiceName.Validation = (o, v) => {
                var value = v.InputString.Replace(" ", "");
                if (value.Length == 0)
                {
                    value = "Lazurite";
                    v.SelectAll = true;
                }
                v.OutputString = value;
            };
            tbPort.TextChanged += (o, e) => SettingsChanged();
            tbServiceName.TextChanged += (o, e) => SettingsChanged();
            btChangeCert.Click += (o, e) => CertificateSelectView.Show(_settings, (s) => SettingsChanged());
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
            _server.StatusChanged += _server_StatusChanged;

            btApply.Click += (o, e) => {
                try
                {
                    _settings.Port = ushort.Parse(tbPort.Text);
                    _settings.ServiceName = tbServiceName.Text;
                    _server.SetSettings(_settings);
                    Lazurite.Windows.Server.Utils.NetshAddSslCert(_settings.CertificateHash, _settings.Port);
                    Lazurite.Windows.Server.Utils.NetshAddUrlacl(_settings.GetAddress());
                    _server.Restart(null);
                    btApply.IsEnabled = false;
                }
                catch (Exception exception)
                {
                    _warningHandler.Error("Во время применения настроек сервера произошла ошибка", exception);
                }
            };
            Refresh();
        }

        private void _server_StatusChanged(LazuriteServer obj)
        {
            this.Dispatcher.BeginInvoke(new Action(() => UpdateServerInfo()));
        }

        public void UpdateServerInfo()
        {
            lblCurrentAddress.Content = _server.GetSettings().GetAddress();
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
            tbServiceName.Text = _settings.ServiceName;
            btApply.IsEnabled = false;
        }

        public void Dispose()
        {
            _server.StatusChanged -= _server_StatusChanged;
        }
    }
}
