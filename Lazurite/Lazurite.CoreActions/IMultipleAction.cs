using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions
{
    public interface IMultipleAction
    {
        IAction[] GetAllActionsFlat();
    }
}
