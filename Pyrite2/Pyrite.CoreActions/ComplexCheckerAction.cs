using Pyrite.ActionsDomain;
using Pyrite.CoreActions.CheckerLogicalOperators;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.ActionsDomain.Attributes;

namespace Pyrite.CoreActions
{
    [VisualInitialization]
    [OnlyGetValue]
    [HumanFriendlyName("СложноеУсловие")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class ComplexCheckerAction : IMultipleAction, IAction, IChecker
    {
        public ComplexCheckerAction()
        {
            CheckerOperations = new List<CheckerOperatorPair>();
        }

        public string Caption
        {
            get
            {
                return string.Empty;
            }
            set
            {
                //
            }
        }
        
        private bool IncrementBool(bool? result, bool value, bool or)
        {
            if (result == null)
                result = value;
            else
            {
                if (or)
                    result |= value;
                else result &= value;
            }
            return result.Value;
        }

        private ToggleValueType _valueType = new ToggleValueType();
        public ActionsDomain.ValueTypes.AbstractValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                //do nothing
            }
        }

        public List<CheckerOperatorPair> CheckerOperations { get; set; }

        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public IAction[] GetAllActionsFlat()
        {
            return CheckerOperations
                .Select(x => (IAction)x.Checker)
                .Union(CheckerOperations.Where(x=>x is IMultipleAction)
                .Select(x=>((IMultipleAction)x)
                .GetAllActionsFlat())
                .SelectMany(x=>x))
                .ToArray();
        }

        public void Initialize()
        {
            //do nothing
        }
        
        public bool Evaluate(ExecutionContext context)
        {
            return GetValue(context) == ToggleValueType.ValueON;
        }

        public void UserInitializeWith<T>() where T : AbstractValueType
        {
            //do nothing
        }

        public string GetValue(ExecutionContext context)
        {
            bool? result = null;

            foreach (var operation in CheckerOperations)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                switch (operation.Operator)
                {
                    case (LogicalOperator.And):
                        result = IncrementBool(result, operation.Checker.Evaluate(context), false);
                        break;
                    case (LogicalOperator.AndNot):
                        result = IncrementBool(result, !operation.Checker.Evaluate(context), false);
                        break;
                    case (LogicalOperator.Or):
                        result = IncrementBool(result, operation.Checker.Evaluate(context), true);
                        break;
                    case (LogicalOperator.OrNot):
                        result = IncrementBool(result, !operation.Checker.Evaluate(context), true);
                        break;
                }
            }

            return result == null || !result.Value ? ToggleValueType.ValueOFF : ToggleValueType.ValueON;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }
    }
}