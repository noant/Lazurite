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
        public void ModulesTest()
        {
            Singleton.Add(new FileSavior());
            var modulesManager = new PluginsManager();
        }

        [TestMethod]
        public void RemoveLibTest()
        {
            Singleton.Add(new FileSavior());
            var modulesManager = new PluginsManager();
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part1()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var modulesManager = new PluginsManager();
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_part2()
        {
            var savior = new FileSavior();
            Singleton.Add(savior);
            var modulesManager = new PluginsManager();
            var testAction = savior.Get<IAction>("testAction");
            if (!testAction.GetType().Equals(modulesManager.GetModules().First()))
                throw new Exception();
        }
        
        [TestMethod]
        public void TestExtModulesAcrossSerializing_experiment1()
        {
            var assembly = Assembly.LoadFrom(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\Pyrite2TestModules.dll");
            var type = assembly.DefinedTypes.First();
            var instance = type.GetConstructor(new Type[0]).Invoke(new object[0]);
            Debug.WriteLine(instance.GetType().FullName);
            var savior = new FileSavior();
            savior.Set("testexp1", instance);
        }

        [TestMethod]
        public void TestExtModulesAcrossSerializing_experiment2()
        {
            var assembly = Assembly.LoadFrom(@"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\Pyrite2TestModules.dll");
            var type = assembly.DefinedTypes.First();
            var savior = new FileSavior();
            var instance = savior.Get<object>("testexp1");
            Debug.WriteLine(instance.GetType().FullName);
        }

        [TestMethod]
        public void TestCreatePluginPackage()
        {
            var targetFile = @"D:\Temporary\pyrite_test.pyp";
            var fromFolder = @"D:\Programming\Pyrite_2\Pyrite2TestModules\Pyrite2TestModules\Pyrite2TestModules\bin\Debug\";
            if (File.Exists(targetFile))
                File.Delete(targetFile);
            Windows.Modules.Utils.CreatePackage(fromFolder, targetFile);
            if (!File.Exists(targetFile))
                throw new Exception();
        }

        [TestMethod]
        public void TestLoadDllTwice()
        {
            TestCreatePluginPackage();
            var targetFile1 = @"D:\Temporary\pyrite_test.pyp";
            var targetFile2 = @"D:\Temporary\pyrite_test2.pyp";
            File.Copy(targetFile1, targetFile2);
            var targetDir = @"D:\Temporary\";
            PluginsCreator.ExtractPluginFile(targetFile1, targetDir);
            PluginsCreator.ExtractPluginFile(targetFile2, targetDir);
            Assembly.LoadFrom(@"D:\Temporary\pyrite_test\Pyrite2TestModules.dll");
            Assembly.LoadFrom(@"D:\Temporary\pyrite_test2\Pyrite2TestModules.dll");
        }
    }
}
