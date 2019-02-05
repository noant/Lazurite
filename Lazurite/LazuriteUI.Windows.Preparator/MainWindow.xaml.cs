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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            Loaded += (o, e) => Task.Factory.StartNew(() => PrepareLazurite(desktopUser));
        }

        private void PrepareLazurite(string desktopUser)
        {
            var log = Singleton.Resolve<WarningHandlerBase>();

            var error = false;

            void appendTb(string text)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    tbInfo.Text += "\r\n" + text;
                }));
            }

            void appendError(string msg, Exception e = null)
            {
                appendTb(msg);
                log.Error(msg, e);
                error = true;
            }

            void appendInfo(string msg)
            {
                appendTb(msg);
                log.Info(msg);
            }

            try
            {
                Dispatcher.BeginInvoke(new Action(() => captionView.StartAnimateProgress()));

                // Prepare environment
                var fileDataManager = Singleton.Resolve<FileDataManager>();
                var assemblyFolder = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(App).Assembly);

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
                        if (files.Length >= 8)
                        {
                            var oldFiles = files
                                .OrderBy(x => x.Name)
                                .OrderBy(x => x.LastWriteTime)
                                .Take(files.Length - 6);

                            foreach (var oldFile in oldFiles)
                                File.Delete(oldFile.FullName);
                        }
                    }
                    else
                        Directory.CreateDirectory(backupFolder);

                    var dateTimeNowSafeString = DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss");
                    var zip = new FastZip();
                    zip.CreateZip(Path.Combine(backupFolder, dateTimeNowSafeString + "_data_backup.zip"), dataFolder, true, string.Empty);
                    zip.CreateZip(Path.Combine(backupFolder, dateTimeNowSafeString + "_plugins_backup.zip"), pluginsFolder, true, string.Empty);
                }
                catch (Exception e)
                {
                    appendError("!Ошибка при создании/удалении файлов бэкапов.", e);
                }

                if (!Utils.VcRedistInstallAll())
                    appendError("!Ошибка установки пакетов VcRedist.");
                else
                    appendInfo("Пакеты VcRedist установлены.");

                try
                {
                    if (!fileDataManager.Has(LazuriteServer.SettingsKey))
                    {
                        // Certificate installing
                        var settingsStub = new ServerSettings();
                        settingsStub.CertificateHash = SecurityHelper.AddCertificateInWindows(Path.Combine(assemblyFolder, CertificateFilename), "28021992");
                        fileDataManager.Set(LazuriteServer.SettingsKey, settingsStub);
                        appendInfo("Настройки сервера созданы.");
                    }
                }
                catch (Exception e)
                {
                    appendError("!Ошибка при создании настроек сервера.", e);
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
                    // When task has been added or changed, it immediately starts the launcher
                    appendInfo($"Запуск Lazurite...");
                    Thread.Sleep(1500); // Extra crutch;
                }
                catch (Exception e)
                {
                    appendError($"!Невозможно добавить программу в автозапуск для пользователем [{desktopUser}].\r\n" +
                        $"Программа не будет запускаться автоматически при логине пользователя [{desktopUser}].", e);
                }

                Dispatcher.BeginInvoke(new Action(() => btOk.IsEnabled = true));
                Dispatcher.BeginInvoke(new Action(() => captionView.StopAnimateProgress()));

                if (!error)
                    Dispatcher.BeginInvoke(new Action(() => App.Current.Shutdown()));
            }
            catch (Exception e)
            {
                appendError("Непредвиденная ошибка! " + e.Message, e);
            }
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}