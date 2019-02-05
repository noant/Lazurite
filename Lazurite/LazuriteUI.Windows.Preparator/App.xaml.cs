using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Windows.Logging;
using System.Security.Principal;
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

            Singleton.Add(new WarningHandler());
            Singleton.Add(new DataEncryptor());
            Singleton.Add(new FileDataManager());
            Singleton.Add(new ScenariosRepository()); // Stub for pluginsManager
            Singleton.Add(new UsersRepository()); // Stub for pluginsManager

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
                Lazurite.Windows.Utils.Utils.ExecuteProcess(
                    Lazurite.Windows.Utils.Utils.GetAssemblyPath(typeof(App).Assembly), 
                    WindowsIdentity.GetCurrent().Name, 
                    true, 
                    false);

                App.Current.Shutdown();
            }
        }
    }
}
