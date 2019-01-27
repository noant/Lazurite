using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using System;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public interface IConstructorElement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<IConstructorElement> NeedAddNext;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<IConstructorElement> NeedRemove;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<IConstructorElement> Modified;
        bool EditMode { get; set; }
        ActionHolder ActionHolder { get; }
        void Refresh(ActionHolder target, IAlgorithmContext aglorithmContext);
        IAlgorithmContext AlgorithmContext { get; }
    }
}
