using Lazurite.MainDomain.MessageSecurity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Lazurite.Tests
{
    [TestClass]
    public class SecureEncodingTest
    {
        [TestMethod]
        public void SecureEncoding_test1()
        {
            var secureEncoding = new SecureEncoding();
            var result = secureEncoding.Encrypt("");
            Debug.WriteLine(result);
            result = secureEncoding.Encrypt("1");
            Debug.WriteLine(result);
        }
    }
}
