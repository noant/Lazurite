using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
