using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using PyriteUI.Icons;

namespace PyriteUI.Tests
{
    [TestClass]
    public class IconsTests
    {
        [TestMethod]
        public void TestGenerateIconsEnums()
        {
            Debug.Write(GenerateEnums.GenerateString());
        }
    }
}
