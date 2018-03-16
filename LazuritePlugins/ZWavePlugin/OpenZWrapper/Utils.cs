using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public static class Utils
    {
        public static Range GetRangeFor(ValueType type)
        {
            switch (type)
            {
                case ValueType.Byte:
                    return new Range(byte.MinValue, byte.MaxValue);
                case ValueType.Decimal:
                    return new Range(decimal.MinValue, decimal.MaxValue);
                case ValueType.Int:
                    return new Range(int.MinValue, int.MaxValue);
                case ValueType.Short:
                    return new Range(short.MinValue, short.MaxValue);
            }
            return new Range();
        }
    }
}
