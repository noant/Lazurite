using Lazurite.Utils;
using System;

namespace Lazurite.MainDomain
{
    public interface ISystemUtils
    {
        void Sleep(int ms, SafeCancellationToken cancelToken);

        SafeCancellationToken StartTimer(Action<SafeCancellationToken> tick, Func<int> needInterval, bool startImmidiate = true, bool ticksSuperposition = false);

        byte[] CreateMD5Hash(byte[] bytes);

        string CurrentLazuriteVersion { get; }
    }
}