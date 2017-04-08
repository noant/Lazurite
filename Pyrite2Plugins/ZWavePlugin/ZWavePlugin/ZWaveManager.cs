using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWavePlugin
{
    public class ZWaveManager
    {
        public static readonly OpenZWrapper.ZWaveManager Current = new OpenZWrapper.ZWaveManager();
    }
}
