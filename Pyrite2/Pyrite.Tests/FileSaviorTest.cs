using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class FileSaviorTest
    {
        public class TestSave
        {
            public int A { get; set; }
            public string B { get; set; }
        }

        [TestMethod]
        public void TestCreateObj()
        {
            var testObj = new TestSave()
            {
                A = 222,
                B = "333"
            };
            new FileSavior().Set("test", testObj);
        }

        [TestMethod]
        public void TestLoadObj()
        {
            var testObj = new FileSavior().Get<TestSave>("test");
            if (testObj.A != 222 || testObj.B != "333")
                throw new Exception();
        }
    }
}
