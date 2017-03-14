using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public interface IAction
    {
        string GetValue(ExecutionContext context);
        void SetValue(ExecutionContext context, string value);
        string Caption { get; set; }
        ValueTypeBase ValueType { get; set; }
        void Initialize();
        void UserInitializeWith(ValueTypeBase valueType);
        ValueChangedDelegate ValueChanged { get; set; }
    }
}
