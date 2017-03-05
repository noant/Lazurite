using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public class OutputChangedDelegates: List<Action<string>>
    {
        public void Execute(string output)
        {
            foreach (var action in this)
                action(output);
        }
    }
}
