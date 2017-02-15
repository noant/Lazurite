using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.ComparisonTypes
{
    public interface IComparisonType
    {
        string Caption { get; set; }
        bool OnlyForNumbers { get; }
        bool Calculate(ActionsDomain.IAction val1, ActionsDomain.IAction val2);
    }
}
