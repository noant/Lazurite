using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.ComparisonTypes
{
    public class EqualityComparisonType : IComparisonType
    {
        public string Caption
        {
            get
            {
                return "=";
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
                return false;
            }
        }

        public bool Calculate(string val1, string val2)
        {
            return val1.Equals(val2);
        }
    }
}
