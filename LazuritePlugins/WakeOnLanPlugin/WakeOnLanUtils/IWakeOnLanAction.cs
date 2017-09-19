using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeOnLanUtils
{
    public interface IWakeOnLanAction
    {
        string MacAddress { get; set; }

        ushort Port { get; set; }

        ushort TryCount { get; set; }
    }
}
