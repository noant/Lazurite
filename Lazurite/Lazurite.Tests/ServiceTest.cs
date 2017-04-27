using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Windows.ServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Tests
{
    [TestClass]
    public class ServiceTest
    {
        [TestMethod]
        public void ClientTest() {
            var client = new ServiceClientFactory().GetServer("localhost", 44340, "LazuriteService.svc", "secretKey1234567", "UserTest_1", "pass_1");
            var a = client.CalculateScenarioValue(new Encrypted<string>("123", "secretKey1234567")).Decrypt("secretKey1234567");
        }
    }
}
