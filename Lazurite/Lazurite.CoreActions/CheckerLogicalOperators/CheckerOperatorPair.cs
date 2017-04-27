using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.CheckerLogicalOperators
{
    public class CheckerOperatorPair
    {
        public IChecker Checker { get; set; }
        public LogicalOperator Operator { get; set; }
    }
}
