using Lazurite.MainDomain.MessageSecurity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
