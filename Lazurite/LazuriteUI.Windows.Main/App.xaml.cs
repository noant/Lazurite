using Lazurite.IOC;
using Lazurite.Windows.Core;
using Lazurite.Windows.Logging;
using LazuriteUI.Windows.Main.Journal;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public LazuriteCore Core { get; private set; }
        
        public App()
        {
            AppDomain.CurrentDomain.ProcessExit += (o, e) => {
                Core.WarningHandler.Info("Lazurite отключен");
            };

            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            System.Windows.Forms.Application.ThreadException += (o, e) => {
                HandleUnhandledException(e.Exception);
            };
            
            AppDomain.CurrentDomain.UnhandledException += (o, e) => {
                HandleUnhandledException(e.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (o, e) => {
                HandleUnhandledException(e.Exception);
            };
            
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                Core = new LazuriteCore();

                //Crutch: after this actions first window runs faster
                JournalLightWindow.Show("Lazurite запущен...", WarnType.Info);
                JournalLightWindow.CloseWindow();

                Core.WarningHandler.OnWrite += (o, e) =>
                {
                    var args = (WarningEventArgs)e;
                    JournalManager.Set(args.Message, args.Value, args.Exception);
                };
                Core.Initialize();
                Core.Server.StartAsync(null);
                Singleton.Add(Core);
                NotifyIconManager.Initialize();
                DuplicatedProcessesListener.Found += (o, e) => NotifyIconManager.ShowMainWindow();
                DuplicatedProcessesListener.Start();
                RightSideHoverForm.Initialize();
            }
            catch (Exception e)
            {
                if (Core != null)
                    Core.WarningHandler.Fatal("Во время инициализации приложения возникла ошибка", e);
                else throw e;
            }

            var ci = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private void HandleUnhandledException(Exception exception)
        {
            WarningHandler.ExtremeLog("Unhandled exception!", exception);

            if (exception != null)
                Core.WarningHandler.FatalFormat(exception, "Необработанная ошибка");
            else
                Core.WarningHandler.FatalFormat(new Exception("unknown exception"), "Необработанная неизвестная ошибка");

            System.Windows.Application.Current.Shutdown(1);
        }
    }
}