using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using System.Diagnostics;
using Pyrite.MainDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.Utils;
using System.Reflection;

namespace Pyrite.Tests
{
    [TestClass]
    public class ReflectionHelperTest
    {
        [TestMethod]
        public void HumanFriendlyName()
        {
            var @var = new ToggleValueType();
            var name = ActionsDomain.Utils.ExtractHumanFriendlyName(var.GetType());
            Debug.WriteLine(name);
            if (name != "Переключатель")
                throw new Exception();
        }

        [TestMethod]
        public void IsOnlyExecute()
        {
            ValueTypeBase @var = new ButtonValueType();
            var isOnlyExec = ActionsDomain.Utils.IsOnlyExecute(var.GetType());
            if (!isOnlyExec)
                throw new Exception();

            @var = new InfoValueType();
            isOnlyExec = ActionsDomain.Utils.IsOnlyExecute(var.GetType());
            if (isOnlyExec)
                throw new Exception();
        }

        [TestMethod]
        public void IsOnlyGetValue()
        {
            ValueTypeBase @var = new ButtonValueType();
            var isOnlyGet = ActionsDomain.Utils.IsOnlyGetValue(var.GetType());
            if (isOnlyGet)
                throw new Exception();

            @var = new InfoValueType();
            isOnlyGet = ActionsDomain.Utils.IsOnlyGetValue(var.GetType());
            if (!isOnlyGet)
                throw new Exception();
        }

        [TestMethod]
        public void GetAllComparisonTypesTest()
        {
            var list = CoreActions.Utils.GetComparisonTypes();
            foreach (var type in list)
                Debug.WriteLine(type.Caption+" "+type.OnlyForNumbers);
        }        
    }
}