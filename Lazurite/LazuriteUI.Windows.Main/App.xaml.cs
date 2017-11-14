using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Windows.Core;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lazurite.Windows.Logging;
using System.Windows.Controls;
using LazuriteUI.Windows.Controls;
using static LazuriteUI.Windows.Main.TestWindow;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Main.Journal;
using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.Security;
using Lazurite.Data;
using System.IO;
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

            this.DispatcherUnhandledException += (o, e) => {
                HandleUnhandledException(e.Exception);
            };
            
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Core = new LazuriteCore();
            Core.WarningHandler.OnWrite += (o, e) => {
                JournalManager.Set(e.Message, e.Type, e.Exception);
            };
            try
            {
                Core.Initialize();
                Core.Server.StartAsync(null);
                Singleton.Add(Core);
            }
            catch (Exception e)
            {
                Core.WarningHandler.Fatal("Во время инициализации приложения возникла ошибка", e);
            }
            NotifyIconManager.Initialize();
            DuplicatedProcessesListener.Found += (processes) => NotifyIconManager.ShowMainWindow();
            DuplicatedProcessesListener.Start();
        }

        private void HandleUnhandledException(Exception exception)
        {
            if (exception != null)
                Core.WarningHandler.FatalFormat(exception, "Необработанная ошибка");
            else
                Core.WarningHandler.FatalFormat(new Exception("unknown exception"), "Необработанная неизвестная ошибка");
            System.Windows.Application.Current.Shutdown(1);
        }
    }
}