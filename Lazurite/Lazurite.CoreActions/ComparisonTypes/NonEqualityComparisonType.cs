using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions.ComparisonTypes
{
    public class NonEqualityComparisonType : IComparisonType
    {
        public string Caption
        {
            get
            {
                return "!=";
            }
            set
            {
                //
            }
        }

        public bool OnlyNumeric
        {
            get
            {
                return false;
            }
        }

        public bool Calculate(IAction val1, IAction val2, ExecutionContext context)
        {
            return !val1?.GetValue(context)?.Equals(val2?.GetValue(context)) ?? true;
        }
    }
}
