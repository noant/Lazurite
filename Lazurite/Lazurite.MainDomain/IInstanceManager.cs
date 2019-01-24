using Lazurite.ActionsDomain;
using System;

namespace Lazurite.MainDomain
{
    public interface IInstanceManager
    {
        IAction CreateInstance(Type actionType, IAlgorithmContext algorithmContext);
        IAction PrepareInstance(IAction action, IAlgorithmContext algorithmContext);
    }
}
