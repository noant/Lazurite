using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions
{
    [OnlyGetValue]
    [VisualInitialization]
    [HumanFriendlyName("Условие")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class CheckerAction : IAction, IMultipleAction, IChecker
    {
        public IAction TargetAction1 { get; set; } = new EmptyAction();
        public IAction TargetAction2 { get; set; } = new EmptyAction();
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
        public ValueTypeBase ValueType
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

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
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

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public bool Evaluate(ExecutionContext context)
        {
            return GetValue(context) == ToggleValueType.ValueON;
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
