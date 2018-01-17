using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public interface IInstanceManager
    {
        IAction CreateInstance(Type actionType, IAlgorithmContext algorithmContext);
        IAction PrepareInstance(IAction action, IAlgorithmContext algorithmContext);
    }
}
