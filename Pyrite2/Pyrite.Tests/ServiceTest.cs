using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var client = new ServiceClientFactory().GetServer("https://localhost:44340/PyriteService.svc", "UserTest_1", "pass_1");
            var a = client.CalculateScenarioValue("123");
        }
    }
}
