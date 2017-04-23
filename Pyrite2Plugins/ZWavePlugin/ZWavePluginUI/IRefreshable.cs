using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWavePluginUI
{
    public interface IRefreshable
    {
        Action<bool> IsDataAllowed { get; set; }
        void Refresh();
    }
}
