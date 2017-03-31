using OpenZWaveDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    internal static class Helper
    {
        public static bool SetValueSucceed(ZWManager manager, ZWValueID valueId, object value)
        {
            return ((dynamic)manager).SetValue(valueId, value); //mega crutch
        }
    }
}
