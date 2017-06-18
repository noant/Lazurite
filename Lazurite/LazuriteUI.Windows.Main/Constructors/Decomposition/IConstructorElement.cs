using Lazurite.ActionsDomain;
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
        bool EditMode { get; set; }
        IAction Action { get; }
    }
}
