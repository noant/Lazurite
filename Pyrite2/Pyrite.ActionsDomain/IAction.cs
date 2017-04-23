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
        bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues);
        event ValueChangedDelegate ValueChanged;
        bool IsSupportsEvent { get; }
    }
}
