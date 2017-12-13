using System;

namespace LazuriteUI.Windows.Main
{
    public interface IAllowSave
    {
        void Save(Action callback);
    }
}
