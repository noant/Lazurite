using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Scenarios.ScenarioTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using System.Threading;
using Pyrite.CoreActions.CheckerLogicalOperators;
using Pyrite.CoreActions.ComparisonTypes;
using System.Diagnostics;

namespace Pyrite.Tests
{
    [TestClass]
    public class ScenariosRepositoryTest
    {
        [TestMethod]
        public void CreateAndSaveScenario()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            var testScen = new SingleActionScenario();
            testScen.Name = "testScen";
            testScen.Category = "category1";
            testScen.TargetAction = new ComplexCheckerActionTest.TestAction() {
                Value = "20"
            };
            repository.AddScenario(testScen);
        }

        [TestMethod]
        public void LoadScenario()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            var testScen = repository.Scenarios.Single(x => x.Name.Equals("testScen"));
            testScen.Execute("30", new System.Threading.CancellationToken());
        }

        [TestMethod]
        public void CreateCompositeScenario()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            Singleton.Add(repository);
            
            var scen = new CompositeScenario();
            scen.TargetAction = new ComplexAction()
            {
                Actions = new List<IAction>()
                {
                    new WhileAction()
                    {
                        Checker = new ComplexCheckerAction()
                        {
                            CheckerOperations = new List<CheckerOperatorPair>() {
                                new CheckerOperatorPair() {
                                    Operator = LogicalOperator.Or,
                                    Checker = new CheckerAction()
                                    {
                                        ComparisonType = new EqualityComparisonType(),
                                        TargetAction1 = new AlwaysOnAction(),
                                        TargetAction2 = new AlwaysOnAction()
                                    }
                                }
                            }
                        },
                        Action = new ComplexAction()
                        {
                            Actions = new List<IAction>
                            {
                                new ExecuteAction()
                                {
                                    Action = new WriteDataAction(),
                                    InputValue = new GetCurrentDateTimeAction()
                                },
                                new ExecuteAction()
                                {
                                    Action = new WaitAction()
                                }
                            }
                        }
                    }
                }
            };
            repository.AddScenario(scen);
            scen.Execute(string.Empty, new CancellationToken());
        }

        public class WaitAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "wa";
                }

                set
                {
                    //
                }
            }

            public AbstractValueType ValueType
            {
                get
                {
                    return new ButtonValueType();
                }
                set
                {
                    //
                }
            }

            public string GetValue(ActionsDomain.ExecutionContext context)
            {
                return string.Empty;
            }

            public void Initialize()
            {
                //
            }

            public void SetValue(ActionsDomain.ExecutionContext context, string value)
            {
                Thread.Sleep(8000);
            }

            public void UserInitializeWith(AbstractValueType valueType)
            {
                //
            }

            public ValueChangedDelegate ValueChanged { get; set; }
        }
        public class GetCurrentDateTimeAction: IAction
        {
            public string Caption
            {
                get
                {
                    return "curdt";
                }
                set
                {
                    //
                }
            }

            public AbstractValueType ValueType
            {
                get
                {
                    return new InfoValueType();
                }

                set
                {
                    //
                }
            }

            public string GetValue(ActionsDomain.ExecutionContext context)
            {
                return DateTime.Now.ToString();
            }

            public void Initialize()
            {
                //
            }

            public void SetValue(ActionsDomain.ExecutionContext context, string value)
            {
                //
            }

            public void UserInitializeWith(AbstractValueType valueType)
            {
                //
            }

            public ValueChangedDelegate ValueChanged { get; set; }
        }
        public class WriteDataAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "wda";
                }

                set
                {
                    //
                }
            }

            public AbstractValueType ValueType
            {
                get
                {
                    return new InfoValueType();
                }

                set
                {
                    //
                }
            }

            public string GetValue(ActionsDomain.ExecutionContext context)
            {
                return "";
            }

            public void Initialize()
            {
                //
            }

            public void SetValue(ActionsDomain.ExecutionContext context, string value)
            {
                Debug.WriteLine(value);
            }

            public void UserInitializeWith(AbstractValueType valueType)
            {
                //
            }

            public ValueChangedDelegate ValueChanged { get; set; }
        }
        public class AlwaysOnAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "aaa";
                }

                set
                {
                    //
                }
            }

            public AbstractValueType ValueType
            {
                get
                {
                    return new ToggleValueType();
                }

                set
                {
                    //
                }
            }

            public string GetValue(ActionsDomain.ExecutionContext context)
            {
                return ToggleValueType.ValueON;
            }

            public void Initialize()
            {
                //
            }

            public void SetValue(ActionsDomain.ExecutionContext context, string value)
            {
                //
            }

            public void UserInitializeWith(AbstractValueType valueType)
            {
                //
            }

            public ValueChangedDelegate ValueChanged { get; set; }
        }
    }
}
