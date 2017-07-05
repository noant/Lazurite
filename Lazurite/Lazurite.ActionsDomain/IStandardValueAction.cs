using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain
{
    public interface IStandardValueAction
    {
        string Value { get; set; }
        ValueTypeBase ValueType { get; set; }
    }
}
