using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.ActionsDomain
{
    public interface IAlgorithmContext
    {
        ValueTypeBase ValueType { get; }
    }
}
