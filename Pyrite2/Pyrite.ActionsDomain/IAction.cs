using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public interface IAction
    {
        string Value { get; set; }
        string Caption { get; }
        ValueType ValueType { get; }
        void Initialize();
    }
}
