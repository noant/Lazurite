using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Preparator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string KillLazuriteProcessCommand = "-kill";
        public static readonly string RemoveFromAutorunCommand = "-removeAutorun";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var log = new WarningHandler();
            var savior = new FileSavior();
            Singleton.Add(savior);
            Singleton.Add(log);
            if (e.Args.Length == 1)
            {
                if (e.Args[0] == KillLazuriteProcessCommand)
                {
                    Utils.KillAllLazuriteProcesses();
                    App.Current.Shutdown();
                }
                else if (e.Args[0] == RemoveFromAutorunCommand)
                {
                    TaskSchedulerUtils.RemoveLogonTask();
                    App.Current.Shutdown();
                }
                else
                    new MainWindow(e.Args[0]).Show();
            }
            else
            {
                Lazurite.Windows.Utils.Utils.ExecuteProcess(Lazurite.Windows.Utils.Utils.GetAssemblyPath(typeof(App).Assembly), WindowsIdentity.GetCurrent().Name, true, false);
                App.Current.Shutdown();
            }
        }
    }
}
