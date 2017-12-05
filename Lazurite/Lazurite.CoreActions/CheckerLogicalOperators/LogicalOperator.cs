using Lazurite.ActionsDomain.Attributes;

namespace Lazurite.CoreActions.CheckerLogicalOperators
{
    public enum LogicalOperator
    {
        [HumanFriendlyName("И")]
        And,
        [HumanFriendlyName("И НЕ")]
        AndNot,
        [HumanFriendlyName("ИЛИ")]
        Or,
        [HumanFriendlyName("ИЛИ НЕ")]
        OrNot
    }
}