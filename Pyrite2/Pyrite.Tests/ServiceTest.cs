using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.MainDomain.MessageSecurity;
using Pyrite.Windows.ServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ServiceTest
    {
        [TestMethod]
        public void ClientTest() {
            var client = new ServiceClientFactory().GetServer("localhost", 44340, "PyriteService.svc", "secretKey1234567", "UserTest_1", "pass_1");
            var a = client.CalculateScenarioValue(new Encrypted<string>("123", "secretKey1234567")).Decrypt("secretKey1234567");
        }
    }
}
