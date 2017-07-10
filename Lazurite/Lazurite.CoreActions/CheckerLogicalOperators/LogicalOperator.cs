using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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