using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions.ComparisonTypes
{
    public interface IComparisonType
    {
        string Caption { get; set; }
        bool OnlyNumeric { get; }
        bool Calculate(IAction val1, IAction val2, ExecutionContext context);
    }
}
