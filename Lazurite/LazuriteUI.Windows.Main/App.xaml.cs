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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            var core = new LazuriteCore();

            core.WarningHandler.OnWrite += (o, e) => {
#if DEBUG
                if (e.Exception != null && (e.Type == WarnType.Error || e.Type == WarnType.Fatal))
                    throw e.Exception;
#endif
                JournalManager.Set(e.Message, e.Type, e.Exception);
            };

            core.Initialize();
            Lazurite.Windows.Server.Utils.NetshAddUrlacl(core.Server.GetSettings().GetAddress());
            Lazurite.Windows.Server.Utils.NetshAddSslCert(core.Server.GetSettings().CertificateHash, core.Server.GetSettings().Port);
            core.Server.StartAsync(null);
            Singleton.Add(core);
            //core.UsersRepository.Add(new Lazurite.MainDomain.UserBase()
            //{
            //    Login = "user1",
            //    PasswordHash = CryptoUtils.CreatePasswordHash("pass")
            //});            
        }
    }
}
