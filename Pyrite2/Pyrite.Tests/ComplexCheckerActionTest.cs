using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.CoreActions;
using Pyrite.CoreActions.CheckerLogicalOperators;
using Pyrite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ComplexCheckerActionTest
    {
        public class TestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "test float action";
                }
                set
                {
                    //
                }
            }

            public string Value
            {
                get;
                set;
            }

            public AbstractValueType ValueType
            {
                get
                {
                    return new FloatValueType();
                }
                set
                {

                }
            }

            public string GetValue(ExecutionContext context)
            {
                return Value;
            }

            public void Initialize()
            {
                //
            }

            public void SetValue(ExecutionContext context, string value)
            {
                Value = value;
                Console.WriteLine(value);
            }

            public void UserInitializeWith(AbstractValueType valueType)
            {
                //
            }
        }

        [TestMethod]
        public void TestCheckerAction()
        {
            var checkerAction = new ComplexCheckerAction();
            checkerAction.CheckerOperations.Add(new CheckerOperatorPair() //true
            {
                Operator = LogicalOperator.Or,
                Checker = new CheckerAction()
                {
                    ComparisonType = new MoreOrEqualComparisonType(),
                    TargetAction1 = new TestAction() { Value = 5.ToString() },
                    TargetAction2 = new TestAction() { Value = 5.ToString() }
                },
            });
            checkerAction.CheckerOperations.Add(new CheckerOperatorPair() //false
            {
                Operator = LogicalOperator.And,
                Checker = new ComplexCheckerAction()
                {
                    CheckerOperations = new List<CheckerOperatorPair>() {
                            new CheckerOperatorPair()
                            {
                                Checker = new CheckerAction()
                                {
                                    ComparisonType = new EqualityComparisonType(),
                                    TargetAction1 = new TestAction() { Value = "5" },
                                    TargetAction2 = new TestAction() { Value = "6" },
                                }
                            }
                        }
                }
            });

            var result = checkerAction.GetValue(null);

            Debug.WriteLine(result);

            if (result != ToggleValueType.ValueOFF)
                throw new Exception();
        }
    }
}
