using Lazurite.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lazurite.Tests
{
    [TestClass]
    public class FileDataManagerTest
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
            new FileDataManager().Set("test", testObj);
        }

        [TestMethod]
        public void TestLoadObj()
        {
            var testObj = new FileDataManager().Get<TestSave>("test");
            if (testObj.A != 222 || testObj.B != "333")
                throw new Exception();
        }

        [TestMethod]
        public void RemoveByKeyTest()
        {
            new FileDataManager().Clear("test");
        }
    }
}
