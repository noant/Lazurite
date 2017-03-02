using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ModulesManagerTest
    {
        [TestMethod]
        public void CreatePluginTest()
        {
            Singleton.Add(new FileSavior());
            var sourcePluginFolder = @"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\";
            var targetFile = @"D:\Temporary\pyrite_test.pyp";
            PluginsCreator.CreatePluginFile(sourcePluginFolder, targetFile);
        }

        [TestMethod]
        public void LoadPluginTest()
        {
            Singleton.Add(new FileSavior());
            var modulesManager = new PluginsManager();
            var targetFile = @"D:\Temporary\pyrite_test.pyp";
            modulesManager.AddPlugin(targetFile);
            if (!modulesManager.GetModules().Any(x => x.Name.Contains("TestAction")))
                throw new Exception();
        }

        [TestMethod]
        public void RemoveLibTest()
        {
            Singleton.Add(new FileSavior());
            var modulesManager = new PluginsManager();
            modulesManager.RemovePlugin(modulesManager.GetPlugins().First().Name);
            if (modulesManager.GetModules().Any(x => x.Name.Contains("TestAction")))
                throw new Exception();
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part1()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var manager = new PluginsManager();
            IAction testAction = manager.CreateInstanceOf(manager.GetModules().First());
            testAction.Value = DateTime.Now.ToString();
            savior.Set("testAction", testAction);
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part2()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var modulesManager = new PluginsManager();
            IAction testAction = savior.Get<IAction>("testAction");
            Debug.WriteLine(testAction.Value);
            if (!testAction.GetType().Equals(modulesManager.GetModules().First()))
                throw new Exception();
        }        
    }
}
