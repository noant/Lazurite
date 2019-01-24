using System;

namespace LazuriteMobile.App
{
    public interface IUpdatable
    {
        void UpdateView(Action callback);
    }
}
