using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ComparisonTypes;
using Lazurite.Data;
using Lazurite.Exceptions;
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

namespace Lazurite.Tests
{
    [TestClass]
    public class TriggerTest3
    {
        [TestMethod]
        public void TriggerTest_3()
        {
            if (Directory.Exists("data"))
                Directory.Delete("data", true);
            if (Directory.Exists("plugins"))
                Directory.Delete("plugins", true);
            Singleton.Add(new FileSavior());
            Singleton.Add(new ExceptionsHandler());
            var manager = new PluginsManager();
            var pluginPath = @"D:\Programming\Lazurite_2\Releases\Plugins\ZWavePlugin.pyp";
            manager.AddPlugin(pluginPath);
            ScenariosRepository rep = new ScenariosRepository();
            var scen = new SingleActionScenario();
            scen.TargetAction = manager.CreateInstanceOf(manager.GetModules().First());
            scen.TargetAction.UserInitializeWith(new ToggleValueType(), false);
            rep.AddScenario(scen);

            var lightZWaction = manager.CreateInstanceOf(manager.GetModules().First());
            lightZWaction.UserInitializeWith(new ToggleValueType(), false);

            var trigger = new Trigger();
            trigger.TargetScenarioId = scen.Id;
            trigger.SetScenario(scen);
            trigger.TargetAction = new ComplexAction()
            {
                Actions = new List<ActionsDomain.IAction>()
                {
                    new IfAction()
                    {
                        Checker = new ComplexCheckerAction()
                        {
                            CheckerOperations = new List<CoreActions.CheckerLogicalOperators.CheckerOperatorPair>() {
                                new CoreActions.CheckerLogicalOperators.CheckerOperatorPair()
                                {
                                    Checker = new CheckerAction()
                                    {
                                        TargetAction1 = lightZWaction,
                                        TargetAction2 = new ToggleConstAction(),
                                        ComparisonType = new EqualityComparisonType()
                                    } 
                                }
                            }
                        },
                        ActionIf = new ComplexAction()
                        {
                            Actions = new List<ActionsDomain.IAction>()
                            {
                                new ExecuteAction()
                                {
                                    Action = lightZWaction,
                                    InputValue = new ToggleConstAction(false),
                                }
                            }
                        },
                        ActionElse = new ComplexAction()
                        {
                            Actions = new List<ActionsDomain.IAction>()
                            {
                                new ExecuteAction()
                                {
                                    Action = lightZWaction,
                                    InputValue = new ToggleConstAction(),
                                }
                            }
                        }
                    }
                }
            };
            trigger.Enabled = true;
            rep.AddTrigger(trigger);
        }

        [TestMethod]
        public void TestTrigger_3_2()
        {
            Singleton.Add(new FileSavior());
            Singleton.Add(new ExceptionsHandler());
            var manager = new PluginsManager();
            var rep = new ScenariosRepository();

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
