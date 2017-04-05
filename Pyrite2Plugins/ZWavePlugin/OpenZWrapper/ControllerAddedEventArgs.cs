using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class ControllerAddedEventArgs: EventArgs
    {
        public Controller Controller { get; internal set; }
        public ZWaveManager Manager { get; internal set; }
        public bool Successful { get; internal set; }
    }
}
