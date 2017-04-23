using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using Pyrite.CoreActions.CheckerLogicalOperators;
using Pyrite.CoreActions.ComparisonTypes;
using Pyrite.CoreActions.CoreActions;
using Pyrite.CoreActions.StandartValueTypeActions;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Scenarios.ScenarioTypes;
using Pyrite.Scenarios.TriggerTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Pyrite.Tests.ScenariosRepositoryTest;

namespace Pyrite.Tests
{
    [TestClass]
    public class TriggerTest
    {
        [TestMethod]
        public void CreateTrigger()
        {
            Singleton.Add(new FileSavior());
            Singleton.Add(new ExceptionsHandler());
            var repository = new ScenariosRepository();

            var testScenario = new CompositeScenario();
            testScenario.TargetAction = new ComplexAction();
            testScenario.ValueType = new StateValueType()
            {
                AcceptedValues = new[] { "State1", "State2", "State3" }
            };
            var @while = new WhileAction();
            @while.Checker = new ComplexCheckerAction()
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
                },
            };
            @while.Action = new ComplexAction()
            {
                Actions = new List<IAction>()
                {
                    new WaitAction(),
                    new SetReturnValueAction()
                    {
                        TargetScenarioId = testScenario.Id,
                        InputValue = new GetStateVTAction()
                        {
                            Value = testScenario.ValueType.AcceptedValues[0]
                        }
                    },
                    new WaitAction(),
                    new SetReturnValueAction()
                    {
                        TargetScenarioId = testScenario.Id,
                        InputValue = new GetStateVTAction()
                        {
                            Value = testScenario.ValueType.AcceptedValues[1]
                        }
                    },
                    new WaitAction(),
                    new SetReturnValueAction()
                    {
                        TargetScenarioId = testScenario.Id,
                        InputValue = new GetStateVTAction()
                        {
                            Value = testScenario.ValueType.AcceptedValues[2]
                        }
                    }
                }
            };

            testScenario.TargetAction.Actions.Add(@while);

            //little crutch
            foreach (ICoreAction coreact in testScenario.TargetAction.GetAllActionsFlat().Where(x => x is ICoreAction))
                coreact.SetTargetScenario(testScenario);
            repository.AddScenario(testScenario);

            testScenario.Initialize(repository);

            //trigger

            var testTrigger = new Trigger();
            testTrigger.TargetScenarioId = testScenario.Id;
            testTrigger.SetScenario(testScenario);

            var input = new GetInputValueAction()
            {
                TargetScenarioId = testScenario.Id
            };
            input.SetTargetScenario(testScenario);

            var output = new WriteInFileAction();
            output.UserInitializeWith(input.ValueType, false);

            testTrigger.TargetAction = new ComplexAction()
            {
                Actions = new List<IAction> {
                    new ExecuteAction()
                    {
                        Action = output,
                        InputValue = input
                    }
                }
            };
            foreach (ICoreAction coreact in ((ComplexAction)testTrigger.TargetAction).GetAllActionsFlat().Where(x => x is ICoreAction))
                coreact.SetTargetScenario(testScenario);
            testTrigger.Enabled = true;
            repository.AddTrigger(testTrigger);
            testTrigger.Initialize(repository);

            //testTrigger
        }

        [TestMethod]
        public void TestTrigger()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        public class WriteInFileAction : IAction
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
                    return "wifa";
                }

                set
                {
                    //
                }
            }

            public event ValueChangedDelegate ValueChanged;

            public ValueTypeBase ValueType
            {
                get;
                set;
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
                File.AppendAllText(@"D:\Temporary\triggerTest.txt", value);
            }
            
            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                this.ValueType = valueType;
                return true;
            }
        }
    }
}
