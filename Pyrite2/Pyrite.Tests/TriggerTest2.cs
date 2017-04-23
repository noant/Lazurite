using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Scenarios.ScenarioTypes;
using Pyrite.Scenarios.TriggerTypes;
using Pyrite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pyrite.Tests.ConcretePluginTest;
using static Pyrite.Tests.ScenariosRepositoryTest;

namespace Pyrite.Tests
{
    [TestClass]
    public class TriggerTest2
    {
        [TestMethod]
        public void TriggerTest_2()
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
            ScenariosRepository rep = new ScenariosRepository();
            var scen = new SingleActionScenario();
            scen.TargetAction = manager.CreateInstanceOf(manager.GetModules().First());
            scen.TargetAction.UserInitializeWith(new ToggleValueType(), false);
            rep.AddScenario(scen);
            var trigger = new Trigger();
            trigger.TargetScenarioId = scen.Id;
            trigger.SetScenario(scen);
            trigger.TargetAction = new ExecuteAction()
            {
                Action = new WriteDebugAction(),
                InputValue = new ToggleConstAction()
            };
            trigger.Enabled = true;
            rep.AddTrigger(trigger);
            rep = new ScenariosRepository();

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
