using System;
using System.Collections.Generic;

namespace Lazurite.ActionsDomain
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
