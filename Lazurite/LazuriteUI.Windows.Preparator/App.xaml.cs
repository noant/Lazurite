using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var log = new WarningHandler();
            var savior = new FileSavior();
            Singleton.Add(savior);
            Singleton.Add(log);
            if (Utils.IsAdministrator() && e.Args.Length == 1)
                new MainWindow(e.Args[0]).Show();
            else
            {
                Utils.ExecuteProcess(Utils.GetAssemblyPath(typeof(App).Assembly), WindowsIdentity.GetCurrent().Name, true, false);
                App.Current.Shutdown();
            }
        }
    }
}
