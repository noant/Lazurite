using Lazurite.ActionsDomain.ValueTypes;
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
        public ExecutionContext(IAlgorithmContext algorithmContext, string input, OutputChangedDelegates output, CancellationToken cancellationToken)
        {
            Input = input;
            OutputChanged = output;
            CancellationToken = cancellationToken;
            AlgorithmContext = algorithmContext;
        }

        public IAlgorithmContext AlgorithmContext { get; private set; }
        public string Input { get; private set; }
        public OutputChangedDelegates OutputChanged { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
    }
}
