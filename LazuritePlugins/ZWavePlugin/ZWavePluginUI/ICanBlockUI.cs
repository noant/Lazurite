using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWavePluginUI
{
    public interface ICanBlockUI
    {
        Action<bool> BlockUI { get; set; }
    }
}
