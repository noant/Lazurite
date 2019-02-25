using Lazurite.Utils;
using System;

namespace Lazurite.ActionsDomain
{
    public class ExecutionContext
    {
        public ExecutionContext(IAlgorithmContext algorithmContext, string input, string previousValue, OutputChangedDelegates output, SafeCancellationToken cancellationTokenSource)
        {
            OutputChanged = output;
            CancellationTokenSource = cancellationTokenSource;
            AlgorithmContext = algorithmContext;
            Input = input;
            PreviousValue = previousValue;
        }

        public ExecutionContext(IAlgorithmContext algorithmContext, string input, string previousValue, OutputChangedDelegates output, ExecutionContext parentContext, SafeCancellationToken cancellationTokenSource)
        {
            OutputChanged = output;
            AlgorithmContext = algorithmContext;
            Input = input;
            PreviousValue = previousValue;
            CancellationTokenSource = cancellationTokenSource;
            ParentContext = parentContext;
            ExecutionNesting = ParentContext.ExecutionNesting + 1;
        }

        public IAlgorithmContext AlgorithmContext { get; private set; }
        public string Input { get; set; }
        public string PreviousValue { get; set; }
        public OutputChangedDelegates OutputChanged { get; private set; }
        public SafeCancellationToken CancellationTokenSource { get; private set; }

        public int ExecutionNesting { get; private set; } = 0;
        public ExecutionContext ParentContext { get; private set; }

        public ExecutionContext Find(Predicate<ExecutionContext> predicate)
        {
            var context = ParentContext;
            var success = false;
            while (true)
            {
                if (context == null)
                {
                    return null;
                }

                success = predicate(context);
                if (success)
                {
                    return context;
                }

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