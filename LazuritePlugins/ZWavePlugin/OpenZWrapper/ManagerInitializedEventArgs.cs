using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class ManagerInitializedEventArgs: EventArgs
    {
        public ZWaveManager Manager { get; internal set; }
        public bool Successful {
            get
            {
                return Manager.Initialized;
            }
        }
    }
}
