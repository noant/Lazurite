using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using System.Diagnostics;
using Pyrite.MainDomain;

namespace Pyrite.Tests
{
    [TestClass]
    public class ReflectionHelperTest
    {
        [TestMethod]
        public void HumanFriendlyName()
        {
            var @var = new ToggleValueType();
            var name = ReflectionHelper.ExtractHumanFriendlyName(@var.GetType());
            Debug.WriteLine(name);
            if (name != "Переключатель")
                throw new Exception();
        }

        [TestMethod]
        public void IsOnlyExecute()
        {
            ActionsDomain.ValueType @var = new ButtonValueType();
            var isOnlyExec = ReflectionHelper.IsOnlyExecute(@var.GetType());
            if (!isOnlyExec)
                throw new Exception();

            @var = new InfoValueType();
            isOnlyExec = ReflectionHelper.IsOnlyExecute(@var.GetType());
            if (isOnlyExec)
                throw new Exception();
        }

        [TestMethod]
        public void IsOnlyGetValue()
        {
            ActionsDomain.ValueType @var = new ButtonValueType();
            var isOnlyGet = ReflectionHelper.IsOnlyGetValue(@var.GetType());
            if (isOnlyGet)
                throw new Exception();

            @var = new InfoValueType();
            isOnlyGet = ReflectionHelper.IsOnlyGetValue(@var.GetType());
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