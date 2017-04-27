using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using LazuriteUI.Icons;

namespace LazuriteUI.Tests
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
