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
        AbstractValueType ValueType { get; set; }
        void Initialize();
        void UserInitializeWith<T>() where T : AbstractValueType;
    }
}
