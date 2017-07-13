using Lazurite.ActionsDomain;
using Lazurite.CoreActions.CheckerLogicalOperators;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions.CoreActions;

namespace Lazurite.CoreActions
{
    [VisualInitialization]
    [OnlyGetValue]
    [HumanFriendlyName("Сложное условие")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class ComplexCheckerAction : IMultipleAction, IAction, IChecker
    {
        public ComplexCheckerAction()
        {
            CheckerOperations = new List<CheckerOperatorPair>();
        }

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

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public ActionsDomain.ValueTypes.ValueTypeBase ValueType
        {
            get;
            set;
        } = new ToggleValueType();

        public List<CheckerOperatorPair> CheckerOperations { get; set; }
        
        public IAction[] GetAllActionsFlat()
        {
            return CheckerOperations
                .Select(x => (IAction)x.Checker)
                .Union(CheckerOperations.Where(x=>x.Checker is IMultipleAction)
                .Select(x=>((IMultipleAction)x.Checker)
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

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
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

        public event ValueChangedDelegate ValueChanged;
    }
}