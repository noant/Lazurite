using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.ComparisonTypes
{
    public class LessOrEqualComparisonType : IComparisonType
    {
        public string Caption
        {
            get
            {
                return "<=";
            }
            set
            {
                //
            }
        }

        public bool OnlyForNumbers
        {
            get
            {
                return true;
            }
        }

        public bool Calculate(IAction val1, IAction val2, ExecutionContext context)
        {
            return val1.ValueType is DateTimeValueType ?
                DateTime.Parse(val1.GetValue(context)) <= DateTime.Parse(val2.GetValue(context)) :
                decimal.Parse(val1.GetValue(context)) <= decimal.Parse(val2.GetValue(context));
        }
    }
}
