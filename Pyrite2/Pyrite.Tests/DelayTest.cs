using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class DelayTest
    {
        [TestMethod]
        public void TestDelay()
        {
            var dt1 = DateTime.Now;
            MainDomain.Utils.Sleep();
            var result = DateTime.Now - dt1;
            Debug.WriteLine(result);
            if (result < TimeSpan.FromMilliseconds(100))
                throw new Exception();

            dt1 = DateTime.Now;
            MainDomain.Utils.Sleep(5, new System.Threading.CancellationToken());
            result = DateTime.Now - dt1;
            Debug.WriteLine(result);
            if (result < TimeSpan.FromMilliseconds(500))
                throw new Exception();
        }

        //[TestMethod]
        //public void DelayStressTest()
        //{
        //    for (int i=0; i <= 6000; i++)
        //    {
        //        MainDomain.Utils.Sleep();
        //    }
        //}
    }
}
