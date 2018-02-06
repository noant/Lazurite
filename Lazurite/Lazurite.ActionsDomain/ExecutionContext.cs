using System;
using System.Threading;

namespace Lazurite.ActionsDomain
{
    public class ExecutionContext
    {
        public ExecutionContext(IAlgorithmContext algorithmContext, string input, OutputChangedDelegates output, CancellationTokenSource cancellationTokenSource)
        {
            Input = input;
            OutputChanged = output;
            CancellationTokenSource = cancellationTokenSource;
            AlgorithmContext = algorithmContext;
        }

        public ExecutionContext(IAlgorithmContext algorithmContext, string input, OutputChangedDelegates output, ExecutionContext parentContext)
        {
            Input = input;
            OutputChanged = output;
            AlgorithmContext = algorithmContext;
            ParentContext = parentContext;
            ExecutionNesting = ParentContext.ExecutionNesting + 1;
            CancellationTokenSource = parentContext.CancellationTokenSource;
        }

        public IAlgorithmContext AlgorithmContext { get; private set; }
        public string Input { get; set; }
        public OutputChangedDelegates OutputChanged { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public int ExecutionNesting { get; private set; } = 0;
        public ExecutionContext ParentContext { get; private set; }

        public ExecutionContext Find(Predicate<ExecutionContext> predicate)
        {
            var context = ParentContext;
            var success = false;
            while (true)
            {
                if (context == null)
                    return null;
                success = predicate(context);
                if (success)
                    return context;
                context = context.ParentContext;
            }
        }

        public void CancelAll()
        {
            var rootContext = ParentContext == null ? this : Find(x => x.ParentContext == null);
            rootContext.CancellationTokenSource.Cancel();
        }
    }
}
