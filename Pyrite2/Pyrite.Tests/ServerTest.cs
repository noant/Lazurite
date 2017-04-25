using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.MainDomain.MessageSecurity;
using Pyrite.Scenarios;
using Pyrite.Security;
using Pyrite.Visual;
using Pyrite.Windows.Server;
using Pyrite.Windows.ServiceClient;
using Pyrite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void RunServer()
        {
            Singleton.Add(new FileSavior());
            Singleton.Add(new ExceptionsHandler());
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

            var server = new PyriteServer();
            
            server.Start();

            Thread.Sleep(3000);

            var clientFactory = Singleton.Resolve<IClientFactory>();

            var client = clientFactory.GetServer("desktop", 444, "PyriteService.svc", "secretKey1234567", "anton", "123");

            var a = client.GetScenarioValue(new Encrypted<string>("16b10918-a709-4418-93eb-fa5d3c9b5d20", "secretKey1234567"));
            var b = client.GetScenarioInfo(new Encrypted<string>("16b10918-a709-4418-93eb-fa5d3c9b5d20", "secretKey1234567"));
            var scens = client.GetScenariosInfo();
            var c = b.Decrypt("secretKey1234567");

            while (true)
                Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestNetshCertAdd()
        {
            Windows.Server.Utils.NetshAddSslCert("localhost", 666);
        }

        [TestMethod]
        public void TestAddCert()
        {
            var certName = Windows.Server.Utils.AddCertificate(@"D:\Programming\Pyrite_2\Pyrite2\Pyrite.Tests\bin\Debug\PyriteStandartCertificate.pfx", "1507199215071992");
            if (certName != "localhost")
                throw new Exception();
        }
    }
}
