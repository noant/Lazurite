using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public struct Range
    {
        public Range(decimal min, decimal max) : this()
        {
            Max = max;
            Min = min;
        }

        public decimal Max { get; private set; }
        public decimal Min { get; private set; }
    }
}
