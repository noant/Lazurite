using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using System;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public interface IConstructorElement
    {
        event Action<IConstructorElement> NeedAddNext;
        event Action<IConstructorElement> NeedRemove;
        event Action<IConstructorElement> Modified;
        bool EditMode { get; set; }
        ActionHolder ActionHolder { get; }
        void Refresh(ActionHolder target, IAlgorithmContext aglorithmContext);
        IAlgorithmContext AlgorithmContext { get; }
    }
}
