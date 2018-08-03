using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Modules;
using Lazurite.Windows.Server;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LazuriteUI.Windows.Preparator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string LauncherExeName = "LazuriteUI.Windows.Launcher.exe";
        private static string CertificateFilename = "LazuriteStandardCertificate.pfx";

        public MainWindow(string desktopUser)
        {
            InitializeComponent();
            Icon = BitmapFrame.Create(Icons.Utils.GetIconData(Icons.Icon.Lazurite64));

            var log = Singleton.Resolve<WarningHandlerBase>();
            var fileSavior = Singleton.Resolve<FileSavior>();
            var assemblyFolder = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly);
            captionView.StartAnimateProgress();
            var message = string.Empty;
            var error = false;

            Utils.VcRedistInstallAll();
            Utils.FirewallSettings();

            if (!fileSavior.Has(LazuriteServer.SettingsKey))
            {
                //certificate installing
                try
                {
                    var settingsStub = new ServerSettings();
                    settingsStub.CertificateHash = Lazurite.Windows.Server.Utils.AddCertificate(Path.Combine(assemblyFolder, CertificateFilename), "28021992");
                    fileSavior.Set(LazuriteServer.SettingsKey, settingsStub);
                    Lazurite.Windows.Server.Utils.NetshAddSslCert(settingsStub.CertificateHash, settingsStub.Port);
                    Lazurite.Windows.Server.Utils.NetshAddUrlacl(settingsStub.GetAddress());
                    Lazurite.Windows.Server.Utils.NetshAllowPort(settingsStub.Port);

                    message += "*Стандартный сертификат для HTTPS установлен.\r\n";
                }
                catch (Exception e)
                {
                    var msg = "*!Ошибка при установке стандартного сертификата для HTTPS. Сервер не будет запущен.\r\n";
                    message += msg;
                    log.ErrorFormat(e, msg);
                    error = true;
                }
            }
            
            //plugins installing
            Singleton.Add(new ScenariosRepository()); //stub for pluginsManager
            Singleton.Add(new UsersRepository()); //stub for pluginsManager
            var pluginsFolderPath = Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly), "PluginsToInstall");
            var pluginsFiles = Directory.GetFiles(pluginsFolderPath).Where(x => x.EndsWith(PluginsManager.PluginFileExtension)).ToArray();
            try
            {
                var pluginsManager = new PluginsManager(false);
                foreach (var pluginPath in pluginsFiles)
                {
                    try
                    {
                        pluginsManager.HardReplacePlugin(pluginPath);
                    }
                    catch (Exception e)
                    {
                        var msg = "*!Ошибка при установке плагина\r\n["+pluginPath+"].\r\n";
                        message += msg;
                        log.ErrorFormat(e, msg);
                        error = true;
                    }
                }
                message += "*Плагины установлены.\r\n";
            }
            catch (Exception e)
            {
                var msg = "*!Ошибка при установке плагинов. Часть функционала может быть недоступна.\r\n";
                message += msg;
                log.ErrorFormat(e, msg);
                error = true;
            }

            //autorun installing
            try
            {
                var mainAppName = Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly), LauncherExeName);
                TaskSchedulerUtils.CreateLogonTask(desktopUser, mainAppName);
                message += string.Format("*Программа добавлена в планировщик задач для пользователя [{0}].\r\n", desktopUser);
            }
            catch (Exception e)
            {
                var msg = string.Format("*!Невозможно добавить программу в автозапуск под пользователем [{0}].\r\nПрограмма не будет запускаться автоматически при логине пользователя [{0}].", desktopUser);
                message += msg;
                log.ErrorFormat(e, msg);
                error = true;
            }
            log.Info(message);
            Complete(message, error);
        }

        public void Complete(string message, bool error)
        {
            captionView.StopAnimateProgress();
            btOk.Visibility = Visibility.Visible;
            label.Content = message;
            if (!error)
                App.Current.Shutdown();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}