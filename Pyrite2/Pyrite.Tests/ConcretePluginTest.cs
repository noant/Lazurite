using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using Pyrite.CoreActions.CheckerLogicalOperators;
using Pyrite.CoreActions.ComparisonTypes;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Scenarios.ScenarioTypes;
using Pyrite.Scenarios.TriggerTypes;
using Pyrite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pyrite.Tests.ScenariosRepositoryTest;

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
            var action = manager.CreateInstanceOf(manager.GetModules().First());
            action.UserInitializeWith(new ToggleValueType(), true);
            ScenariosRepository rep = new ScenariosRepository();
            var scenario = new CompositeScenario();
            scenario.TargetAction = new CoreActions.ComplexAction()
            {
                Actions = new List<ActionsDomain.IAction>()
                {
                    new WhileAction()
                    {
                        Checker = new ComplexCheckerAction()
                        {
                            CheckerOperations = new List<CheckerOperatorPair>()
                            {
                                new CheckerOperatorPair()
                                {
                                    Checker = new CheckerAction()
                                    {
                                        ComparisonType = new EqualityComparisonType(),
                                        TargetAction2 = new AlwaysOnAction(),
                                        TargetAction1 = new AlwaysOnAction()
                                    }
                                }
                            }
                        },
                        Action = new ComplexAction()
                        {
                            Actions = new List<ActionsDomain.IAction>()
                            {
                                new IfAction()
                                {
                                    Checker = new ComplexCheckerAction()
                                    {
                                        CheckerOperations = new List<CheckerOperatorPair>()
                                        {
                                            new CheckerOperatorPair()
                                            {
                                                Checker = new CheckerAction()
                                                {
                                                    TargetAction1 = new AlwaysOnAction(),
                                                    TargetAction2 = action,
                                                    ComparisonType = new EqualityComparisonType()
                                                }
                                            }
                                        }
                                    },
                                    ActionIf = new ComplexAction()
                                    {
                                        Actions = new List<IAction>()
                                        {
                                            new ExecuteAction()
                                            {
                                                Action = new WriteDebugAction(),
                                                InputValue = action
                                            }
                                        }
                                    },
                                    ActionElse = new ComplexAction()
                                    {
                                        Actions = new List<IAction>()
                                        {
                                            new ExecuteAction()
                                            {
                                                Action = new WriteDebugAction(),
                                                InputValue = action
                                            }
                                        }
                                    }
                                },
                                new WaitAction()
                            }
                        }                        
                    }
                }
            };
            rep.AddScenario(scenario);
            rep = new ScenariosRepository();
            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        public class WriteDebugAction : IAction
        {
            public bool IsSupportsEvent
            {
                get
                {
                    return ValueChanged != null;
                }
            }

            public string Caption
            {
                get
                {
                    return string.Empty;
                }

                set
                {
                    
                }
            }

            public event ValueChangedDelegate ValueChanged;

            public ValueTypeBase ValueType
            {
                get
                {
                    return new ToggleValueType();
                }

                set
                {

                }
            }

            public string GetValue(ActionsDomain.ExecutionContext context)
            {
                return "true";
            }

            public void Initialize()
            {
            }

            public void SetValue(ExecutionContext context, string value)
            {
                Debug.WriteLine(value);
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }
        public class WaitAction : IAction
        {
            public bool IsSupportsEvent
            {
                get
                {
                    return ValueChanged != null;
                }
            }

            public string Caption
            {
                get;
                set;
            }

            public event ValueChangedDelegate ValueChanged;

            public ValueTypeBase ValueType
            {
                get;

                set;
            }

            public string GetValue(ExecutionContext context)
            {
                return "";
            }

            public void Initialize()
            {
            }

            public void SetValue(ExecutionContext context, string value)
            {
                System.Threading.Thread.Sleep(1000);
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }
    }
}
