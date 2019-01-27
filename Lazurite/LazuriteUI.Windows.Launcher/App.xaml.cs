using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace LazuriteUI.Windows.Launcher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string MainExeName = "LazuriteUI.Windows.Main.exe";
        private static string TaskSchedulerMode = "-FromTaskScheduler";
        private static WarningHandlerBase Log;

        private void HandleUnhandledException(Exception exception)
        {
            WarningHandler.ExtremeLog("Launcher unhandled exception!", exception);
            if (exception != null)
                Log.FatalFormat(exception, "Необработанная ошибка");
            else
                Log.FatalFormat(new Exception("unknown exception"), "Необработанная неизвестная ошибка");
            System.Windows.Application.Current.Shutdown(1);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (o, args) => {
                HandleUnhandledException(args.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (o, args) => {
                HandleUnhandledException(args.Exception);
            };

            Singleton.Add(new FileSavior());
            Singleton.Add(Log = new WarningHandler());
            Log.OnWrite += (sender, eventArgs) =>
            {
                var args = (WarningEventArgs)eventArgs;
                if (args.Value == WarnType.Error || args.Value == WarnType.Fatal)
                    MessageBox.Show(args.Message, args.Exception?.Message);
            };
            if (e.Args.Any() && e.Args[0] == TaskSchedulerMode)
            {
                if (new LauncherSettingsManager().Settings.RunOnUserLogon)
                    RunLazuriteWithAdminPrivileges();
                else Shutdown();
            }
            else
                RunLazuriteWithAdminPrivileges();
        }

        public void RunLazuriteWithAdminPrivileges()
        {
            if (Utils.IsAdministrator())
            {
                RunLazurite(false);
            }
            else
            {
                var requireAdminRightsWindow = new RequireAdminRightsWindow();
                requireAdminRightsWindow.ApplyClick += (o, e) => RunLazurite(true);
                requireAdminRightsWindow.CancelClick += (o, e) => Shutdown();
                requireAdminRightsWindow.Show();
            }
        }

        public void RunLazurite(bool useShell)
        {
            try
            {
                Utils.ExecuteProcess(Path.Combine(Utils.GetAssemblyFolder(typeof(App).Assembly), MainExeName), string.Empty, useShell, false, System.Diagnostics.ProcessPriorityClass.High);
                Shutdown();
            }
            catch (Exception exception)
            {
                Log.Fatal("При запуске Lazurite возникла ошибка.", exception);
            }
        }
    }
}