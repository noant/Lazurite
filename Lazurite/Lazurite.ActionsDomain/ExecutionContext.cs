using System.Threading;

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
        public string Input { get; set; }
        public OutputChangedDelegates OutputChanged { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
    }
}
