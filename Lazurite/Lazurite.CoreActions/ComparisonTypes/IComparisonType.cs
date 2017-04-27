using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.ComparisonTypes
{
    public interface IComparisonType
    {
        string Caption { get; set; }
        bool OnlyForNumbers { get; }
        bool Calculate(IAction val1, IAction val2, ExecutionContext context);
    }
}
