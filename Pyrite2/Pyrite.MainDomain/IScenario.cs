using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public interface IScenario
    {
        string Id { get; } //guid
        void Execute(string param);
        ValueType ValueType { get; }
        void OnStateChanged(Action<IScenario> action);
        void OnStateChanged(Action<IScenario> action, bool onlyOnce);
    }
}
