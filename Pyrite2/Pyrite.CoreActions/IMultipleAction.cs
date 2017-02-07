using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions
{
    public interface IMultipleAction
    {
        IAction GetAllActionsFlat();
    }
}
