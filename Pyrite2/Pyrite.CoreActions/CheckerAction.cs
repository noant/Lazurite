using Pyrite.ActionsDomain;
using Pyrite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions
{
    [OnlyGetValue]
    [VisualInitialization]
    public class CheckerAction : IAction, IMultipleAction, IChecker
    {
        public IAction TargetAction1 { get; set; }
        public IAction TargetAction2 { get; set; }
        public IComparisonType ComparisonType { get; set; }

        public string Caption
        {
            get
            {
                return "Логическое действие";
            }
        }

        public string Value
        {
            get
            {
                return ComparisonType.Calculate(TargetAction1.Value, TargetAction2.Value) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
            }
            set
            {
                //
            }
        }

        private ToggleValueType _valueType = new ToggleValueType();
        public ActionsDomain.ValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                //
            }
        }

        public IAction[] GetAllActionsFlat()
        {
            return new[] { TargetAction1, TargetAction2 };
        }

        public void Initialize()
        {
            //do nothing
        }

        public void UserInitialize()
        {
            //do nothing
        }

        public bool Evaluate()
        {
            return Value == ToggleValueType.ValueON;
        }
    }
}
