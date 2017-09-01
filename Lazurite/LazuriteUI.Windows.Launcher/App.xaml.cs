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

namespace LazuriteUI.Windows.Launcher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string MainExeName = "LazuriteUI.Windows.Main.exe";
        private static WarningHandlerBase Log = new WarningHandler();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.OnWrite += (sender, args) =>
            {
                if (args.Type == WarnType.Error || args.Type == WarnType.Fatal)
                    MessageBox.Show(args.Message, args.Exception?.Message);
            };
            Singleton.Add(Log);
            if (Utils.IsAdministrator())
            {
                RunLazurite(false);
            }
            else
            {
                var requireAdminRightsWindow = new RequireAdminRightsWindow();
                requireAdminRightsWindow.ApplyClick += () => RunLazurite(true);
                requireAdminRightsWindow.CancelClick += () => Shutdown();
                requireAdminRightsWindow.Show();
            }
        }

        public void RunLazurite(bool useShell)
        {
            try
            {
                Utils.ExecuteProcess(Path.Combine(Utils.GetAssemblyFolder(typeof(App).Assembly), MainExeName), string.Empty, useShell, false);
                Shutdown();
            }
            catch (Exception exception)
            {
                Log.Error("При запуске Lazurite возникла ошибка.", exception);
            }
        }
    }
}
