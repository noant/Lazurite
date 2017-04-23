using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ConcretePluginTest
    {
        [TestMethod]
        public void ZWavePluginTest()
        {
            if (Directory.Exists("data"))
                Directory.Delete("data", true);
            if (Directory.Exists("plugins"))
                Directory.Delete("plugins", true);
            Singleton.Add(new FileSavior());
            Singleton.Add(new ExceptionsHandler());
            var manager = new PluginsManager();
            var repository = new ScenariosRepository();
            var pluginPath = @"D:\Programming\Pyrite_2\Releases\Plugins\ZWavePlugin.pyp";
            manager.AddPlugin(pluginPath);
            var types = manager.GetModules();
            var concreteAction = manager.CreateInstanceOf(types.First());
            concreteAction.UserInitializeWith(new FloatValueType());
        }
    }
}
