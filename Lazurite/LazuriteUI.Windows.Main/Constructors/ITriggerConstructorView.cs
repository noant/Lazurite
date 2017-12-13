using Lazurite.MainDomain;
using System;

namespace LazuriteUI.Windows.Main.Constructors
{
    public interface ITriggerConstructorView
    {
        void Revert(TriggerBase scenario);
        event Action Modified;
        event Action Failed;
        event Action Succeed;
    }
}
