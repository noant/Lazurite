using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions.CheckerLogicalOperators
{
    public class CheckerOperatorPair: IAction
    {
        public string Caption
        {
            get
            {
                return string.Empty;
            }

            set
            {
                //do nothing
            }
        }

        public IChecker Checker { get; set; } = new CheckerAction();

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public LogicalOperator Operator { get; set; } = LogicalOperator.And;

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ButtonValueType();

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
