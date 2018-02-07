using System;
using System.Threading;

namespace Lazurite.MainDomain
{
    public interface ISystemUtils
    {
        void Sleep(int ms, CancellationToken cancelToken);

        bool IsFaultException(Exception e);

        bool IsFaultExceptionHasCode(Exception e, string code);

        CancellationTokenSource StartTimer(Action<CancellationTokenSource> tick, Func<int> needInterval, bool startImmidiate = true, bool ticksSuperposition = false);
    }
}
