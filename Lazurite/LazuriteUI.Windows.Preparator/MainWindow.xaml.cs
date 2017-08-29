using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using Lazurite.Windows.Utils;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LazuriteUI.Windows.Preparator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string MainExeName = "LazuriteUI.Windows.Main.exe";
        private static string CertificateFilename = "LazuriteStandardCertificate.pfx";

        public MainWindow()
        {
            InitializeComponent();

            var log = new WarningHandler();
            var savior = new FileSavior();
            Singleton.Add(savior);
            Singleton.Add(log);

            var assemblyFolder = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly);

            captionView.StartAnimateProgress();
            var adminUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            var message = string.Empty;

            try
            {
                var settingsStub = new ServerSettings();
                settingsStub.CertificateHash = Lazurite.Windows.Server.Utils.AddCertificate(Path.Combine(assemblyFolder, CertificateFilename), "28021992");
                savior.Set(LazuriteServer.SettingsKey, settingsStub);
                Lazurite.Windows.Server.Utils.NetshAddSslCert(settingsStub.CertificateHash, settingsStub.Port);
                Lazurite.Windows.Server.Utils.NetshAddUrlacl(settingsStub.GetAddress());
                message += "*Стандартный сертификат для HTTPS установлен.\r\n";
            }
            catch (Exception e)
            {
                var msg = "*!Ошибка при установке стандартного сертификата для HTTPS. Сервер не будет запущен.\r\n";
                message += msg;
                log.ErrorFormat(e, msg);
            }

            try
            {
                var mainAppName = Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly), MainExeName);
                TaskSchedulerUtils.CreateLogonTask(adminUser, mainAppName);
                message += string.Format("*Программа добавлена в планировщик задач для пользователя [{0}].\r\nДля дальнейшей работы программы необходимо перелогиниться под пользователем\r\n[{0}].\r\n", adminUser);
            }
            catch (Exception e)
            {
                var msg = string.Format("*!Невозможно добавить программу в автозапуск под пользователем [{0}].\r\nПрограмма не будет запускаться автоматически при логине пользователя [{0}].", adminUser);
                message += msg;
                log.ErrorFormat(e, msg);
            }
            log.Info(message);
            Complete(message);
        }

        public void Complete(string message)
        {
            captionView.StopAnimateProgress();
            btOk.Visibility = Visibility.Visible;
            label.Content = message;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}