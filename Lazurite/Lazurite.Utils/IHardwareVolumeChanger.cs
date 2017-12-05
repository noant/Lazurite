using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Utils
{
    public interface IHardwareVolumeChanger
    {
        event EventsHandler<int> VolumeUp; 
        event EventsHandler<int> VolumeDown;
    }
}
