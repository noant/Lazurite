using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Visual;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using Lazurite.Windows.ServiceClient;
using Lazurite.Windows.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Tests
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void RunServer()
        {
            Singleton.Add(new FileSavior());
            Singleton.Add(new WarningHandler());
            Singleton.Add(new ScenariosRepository());
            Singleton.Add(new UsersRepository());
            Singleton.Add(new VisualSettingsRepository());
            Singleton.Add(new ServiceClientFactory());
            
            var scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
            var usersRepository = Singleton.Resolve<UsersRepository>();
            if (!usersRepository.Users.Any(x => x.Login.Equals("anton")))
            {
                usersRepository.Add(new User()
                {
                    Login = "anton",
                    PasswordHash = CryptoUtils.CreatePasswordHash("123")
                });
            }

            var server = new LazuriteServer();
            
            server.StartAsync(null);
            
            while (true)
                Thread.Sleep(5000);
        }
        
        [TestMethod]
        public void TestAddCert()
        {
            var certName = Windows.Server.Utils.AddCertificate(@"D:\Programming\Lazurite_2\Lazurite\Lazurite.Tests\bin\Debug\LazuriteStandartCertificate.pfx", "1507199215071992");
            if (certName != "localhost")
                throw new Exception();
        }
    }
}
