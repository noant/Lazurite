using System;

namespace ZWavePluginUI
{
    public interface ICanBlockUI
    {
        Action<bool> BlockUI { get; set; }
    }
}
