using System;

namespace ZWavePluginUI
{
    public interface IRefreshable
    {
        Action<bool> IsDataAllowed { get; set; }
        Action NeedClose { get; set; }
        void Refresh();
    }
}
