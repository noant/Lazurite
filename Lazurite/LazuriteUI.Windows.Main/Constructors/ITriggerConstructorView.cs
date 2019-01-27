using Lazurite.MainDomain;
using System;

namespace LazuriteUI.Windows.Main.Constructors
{
    public interface ITriggerConstructorView
    {
        void Revert(TriggerBase scenario);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action Modified;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action Failed;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action Succeed;
    }
}
