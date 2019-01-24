using System;
using System.Threading;

namespace Lazurite.MainDomain
{
    public interface ISystemUtils
    {
        void Sleep(int ms, CancellationToken cancelToken);

        CancellationTokenSource StartTimer(Action<CancellationTokenSource> tick, Func<int> needInterval, bool startImmidiate = true, bool ticksSuperposition = false);

        byte[] CreateMD5Hash(byte[] bytes);
    }
}
