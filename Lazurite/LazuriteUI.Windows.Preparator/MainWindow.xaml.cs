using ICSharpCode.SharpZipLib.Zip;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Modules;
using Lazurite.Windows.Server;
using SimpleRemoteMethods.Utils.Windows;
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

            captionView.StartAnimateProgress();

            // Prepare environment
            var log = Singleton.Resolve<WarningHandlerBase>();
            var fileDataManager = Singleton.Resolve<FileDataManager>();
            Singleton.Add(new ScenariosRepository()); // Stub for pluginsManager
            Singleton.Add(new UsersRepository()); // Stub for pluginsManager
            var assemblyFolder = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly);
            var message = string.Empty;
            var error = false;

            void appendError(string msg, Exception e = null)
            {
                message += msg + "\r\n";
                log.Error(msg, e);
                error = true;
            }

            void appendInfo(string msg)
            {
                message += msg + "\r\n";
                log.Info(msg);
            }

            // Create data backup
            var dataFolder = Path.Combine(assemblyFolder, "data");
            var pluginsFolder = Path.Combine(assemblyFolder, "plugins");
            var backupFolder = Path.Combine(assemblyFolder, "backup");

            try
            {
                if (Directory.Exists(backupFolder))
                {
                    // Clear old backups
                    var files = new DirectoryInfo(backupFolder).EnumerateFiles().ToArray();
                    if (files.Count() >= 8)
                    {
                        var oldFiles = files
                            .OrderBy(x => x.Name)
                            .OrderBy(x => x.LastWriteTime)
                            .Take(files.Count() - 4);

                        foreach (var oldFile in oldFiles)
                            File.Delete(oldFile.FullName);
                    }
                }
                else
                    Directory.CreateDirectory(backupFolder);

                var dateTimeNowSafeString = DateTime.Now.ToString().Replace(':', '.');
                var zip = new FastZip();
                zip.CreateZip(Path.Combine(backupFolder, dateTimeNowSafeString + "_data_backup.zip"), dataFolder, true, string.Empty);
                zip.CreateZip(Path.Combine(backupFolder, dateTimeNowSafeString + "_plugins_backup.zip"), pluginsFolder, true, string.Empty);
            }
            catch (Exception e)
            {
                appendError("!Ошибка при создании/удалении файлов бэкапов.", e);
            }

            if (!Utils.VcRedistInstallAll())
                appendError("!Ошибка установки файлов VcRedist.");

            try
            {
                if (!fileDataManager.Has(LazuriteServer.SettingsKey))
                {
                    // Certificate installing
                    var settingsStub = new ServerSettings();
                    settingsStub.CertificateHash = SecurityHelper.AddCertificateInWindows(Path.Combine(assemblyFolder, CertificateFilename), "28021992");
                    fileDataManager.Set(LazuriteServer.SettingsKey, settingsStub);
                }
            }
            catch (Exception e)
            {
                appendError("!Ошибка при создании/удалении файлов бэкапов.", e);
            }
            
            // Plugins installing
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
                        appendError($"!Ошибка при установке плагина\r\n[{pluginPath}].", e);
                    }
                }
                appendInfo("Плагины установлены.");
            }
            catch (Exception e)
            {
                appendError("!Ошибка при установке плагинов. Часть функционала может быть недоступна.", e);
            }

            // Autorun installing
            try
            {
                var mainAppName = Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly), LauncherExeName);
                TaskSchedulerUtils.CreateLogonTask(desktopUser, mainAppName);
                appendInfo($"Программа добавлена в планировщик задач для пользователя [{desktopUser}].");
            }
            catch (Exception e)
            {
                appendError($"!Невозможно добавить программу в автозапуск для пользователем [{desktopUser}].\r\n" +
                    $"Программа не будет запускаться автоматически при логине пользователя [{desktopUser}].", e);
            }

            btOk.Visibility = Visibility.Visible;
            label.Content = message;
            captionView.StopAnimateProgress();

            if (!error)
                App.Current.Shutdown();
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}