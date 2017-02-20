using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ModulesManagerTest
    {
        [TestMethod]
        public void ModulesTest()
        {
            Singleton.Add(new FileSavior());

            var modulesManager = new ModulesManager();
            bool isNotLib = false;
            modulesManager.AddLibrary(@"D:\Temporary\auto.txt", out isNotLib);
            if (!isNotLib)
                throw new Exception();
            modulesManager.AddLibrary(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\TestLib.dll", out isNotLib);
            modulesManager.AddLibrary(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\Pyrite2TestModules.dll", out isNotLib);

            if (!modulesManager.GetModules().Any(x => x.Name.Equals("TestAction1"))) {
                throw new Exception();
            }
        }

        [TestMethod]
        public void RemoveLibTest()
        {
            Singleton.Add(new FileSavior());
            var modulesManager = new ModulesManager();

            modulesManager.RemoveLibrary("auto.txt");
            modulesManager.RemoveLibrary("Pyrite2TestModules.dll");
            modulesManager.RemoveLibrary("TestLib.dll");

            if (modulesManager.GetModules().Any(x => x.Name.Equals("TestAction1")))
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part1()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var modulesManager = new ModulesManager();
            bool isNotLib = false;
            modulesManager.AddLibrary(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\TestLib.dll", out isNotLib);
            modulesManager.AddLibrary(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\Pyrite2TestModules.dll", out isNotLib);
            var testAction = modulesManager.CreateInstanceOf(modulesManager.GetModules().First());
            testAction.Value = DateTime.Now.ToString();
            savior.Set("testAction", testAction);
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part2()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var modulesManager = new ModulesManager();
            var testAction = savior.Get<IAction>("testAction");
            if (!testAction.GetType().Equals(modulesManager.GetModules().First()))
                throw new Exception();
        }
    }
}
