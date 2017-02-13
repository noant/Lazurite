using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.ComparisonTypes
{
    public class MoreOrEqualComparisonType : IComparisonType
    {
        public string Caption
        {
            get
            {
                return ">=";
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
        
        public bool Calculate(string val1, string val2)
        {
            return decimal.Parse(val1) >= decimal.Parse(val2);
        }
    }
}
