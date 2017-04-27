using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain
{
    public class ExecutionContext
    {
        public ExecutionContext(string input, OutputChangedDelegates output, CancellationToken cancellationToken)
        {
            Input = input;
            OutputChanged = output;
            CancellationToken = cancellationToken;
        }

        public string Input { get; private set; }
        public OutputChangedDelegates OutputChanged { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
    }
}
