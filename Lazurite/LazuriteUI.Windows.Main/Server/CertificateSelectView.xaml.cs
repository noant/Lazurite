using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Common;
using SimpleRemoteMethods.Utils.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace LazuriteUI.Windows.Main.Server
{
    /// <summary>
    /// Логика взаимодействия для CertificateSelectView.xaml
    /// </summary>
    public partial class CertificateSelectView : System.Windows.Controls.UserControl
    {
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();

        public CertificateSelectView(ServerSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            Refresh();

            btInstallNewCert.Click += (o, e) => {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Файл сертификата (*.pfx,*.cer,*.p7b)|*.pfx;*.cer;*.p7b";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    EnterPasswordView.Show("Введите пароль сертификата...", (pass) => {
                        try
                        {
                            var certHash = SecurityHelper.AddCertificateInWindows(openFileDialog.FileName, pass);
                            Refresh();
                            certListView.GetItems().Where(x => ((ServerHelper.CertificateInfo)((ItemView)x).Tag).Hash.Equals(certHash)).All(x => x.Selected = true);
                        }
                        catch (Exception exception)
                        {
                            _warningHandler.ErrorFormat(exception, "Ошибка при добавлении сертификата [{0}]", openFileDialog.FileName);
                        }
                    }, null, "Если файл сертификата не имеет пароль, то нажмите \"Применить\"");
                }
            };

            certListView.SelectionChanged += (o, e) => btApply.IsEnabled = true;
            btApply.Click += (o, e) =>
            {
                var selectedCert = (ServerHelper.CertificateInfo)((ItemView)certListView.SelectedItem).Tag;
                _settings.CertificateHash = selectedCert.Hash;
                Selected?.Invoke(settings);
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ServerSettings> Selected;

        private ServerSettings _settings;

        public void Refresh()
        {
            certListView.Children.Clear();
            var certs = ServerHelper.GetInstalledCertificates();
            foreach (var cert in certs)
            {
                var itemView = new ItemView();
                itemView.Content = cert.Description;
                itemView.Tag = cert;
                itemView.Margin = new Thickness(2);
                certListView.Children.Add(itemView);
                if (cert.Hash.Equals(_settings.CertificateHash))
                    itemView.Selected = true;
            }
        }

        public static void Show(ServerSettings settings, Action<ServerSettings> selected)
        {
            var selectView = new CertificateSelectView(settings);
            var dialogView = new DialogView(selectView);
            selectView.Selected += new Action<ServerSettings>((s) => {
                selected?.Invoke(s);
                dialogView.Close();
            });
            dialogView.Show();
        }
    }
}
