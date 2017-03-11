using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;
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
    [HumanFriendlyName("Условие")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class CheckerAction : IAction, IMultipleAction, IChecker
    {
        public IAction TargetAction1 { get; set; }
        public IAction TargetAction2 { get; set; }
        public IComparisonType ComparisonType { get; set; }

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
                
        public string GetValue(ExecutionContext context)
        {
            return ComparisonType.Calculate(TargetAction1, TargetAction2, context) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        private ToggleValueType _valueType = new ToggleValueType();
        public AbstractValueType ValueType
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

        public void UserInitializeWith(AbstractValueType valueType)
        {
            //do nothing
        }

        public bool Evaluate(ExecutionContext context)
        {
            return GetValue(context) == ToggleValueType.ValueON;
        }

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}
