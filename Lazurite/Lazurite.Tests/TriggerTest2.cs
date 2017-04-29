using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Scenarios;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Scenarios.TriggerTypes;
using Lazurite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lazurite.Tests.ConcretePluginTest;
using static Lazurite.Tests.ScenariosRepositoryTest;
using Lazurite.Windows.Logging;

namespace Lazurite.Tests
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
            Singleton.Add(new WarningHandler());
            var manager = new PluginsManager();
            var repository = new ScenariosRepository();
            var pluginPath = @"D:\Programming\Lazurite_2\Releases\Plugins\ZWavePlugin.pyp";
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
