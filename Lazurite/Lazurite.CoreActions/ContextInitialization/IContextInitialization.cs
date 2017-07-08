using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.ContextInitialization
{
    public interface IContextInitializable
    {
        void Initialize(IAlgorithmContext parameters);
    }
}
