using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.MainDomain;
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

            //Windows.Server.Utils.NetshAddUrlacl(server.GetSettings());
            //Windows.Server.Utils.NetshAddSslCert(server.GetSettings());

            server.Start();

            Thread.Sleep(3000);

            var clientFactory = Singleton.Resolve<IClientFactory>();

            var client = clientFactory.GetServer("desktop", 444, "PyriteService.svc", "anton", "123");

            var a = client.GetScenarioValue("16b10918-a709-4418-93eb-fa5d3c9b5d20");
            var b = client.GetScenarioInfo("16b10918-a709-4418-93eb-fa5d3c9b5d20");
            var scens = client.GetScenariosInfo();

            while (true)
                Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestNetshCertAdd()
        {
            Windows.Server.Utils.NetshAddSslCert(new ServerSettings()
            {
                CertificateSubject = "localhost",
                Port = 666,
                ServiceName = "test"
            });
        }
    }
}
