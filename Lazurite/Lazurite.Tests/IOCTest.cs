using Lazurite.IOC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lazurite.Tests
{
    [TestClass]
    public class IOCTest
    {
        [TestMethod]
        public void TestSingletonResolver()
        {
            var tic = new TestInterfaceClass();
            Singleton.Add(tic);
            var tic2 = Singleton.Resolve<ITestInterface>();
            var tic3 = Singleton.Resolve<TestInterfaceClass>();

            var at = new ConcreteTest();
            Singleton.Add(at);
            var at2 = Singleton.Resolve<AbstractTest>();
            var at3 = Singleton.Resolve<ConcreteTest>();

            var tc = new TestSubClass();
            Singleton.Add(tc);
            var tc2 = Singleton.Resolve<TestClass>();
            if (tc2.GetInt() != tc.GetInt())
                throw new Exception();

            if (tic2 == null || at2 == null || at3 == null || tc2 == null)
                throw new Exception();
        }
    }

    public interface ITestInterface
    {
        int GetInt();
    }

    public class TestInterfaceClass : ITestInterface
    {
        public int GetInt()
        {
            return 12345;
        }
    }

    public abstract class AbstractTest
    {
        public abstract string GetString();
    }

    public class ConcreteTest : AbstractTest
    {
        public override string GetString()
        {
            return "bla-bla";
        }
    }

    public class TestClass
    {
        public virtual int GetInt()
        {
            return 0;
        }
    }

    public class TestSubClass : TestClass
    {
        public override int GetInt()
        {
            return base.GetInt() + 1;
        }
    }

}
