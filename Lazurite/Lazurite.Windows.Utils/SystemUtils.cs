using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace Lazurite.Windows.Utils
{
    public class SystemUtils : ISystemUtils
    {
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(300);

        public string CurrentLazuriteVersion { get; } = Assembly.GetEntryAssembly().GetName().Version.ToString();

        public byte[] CreateMD5Hash(byte[] bytes) => MD5.Create().ComputeHash(bytes);

        public void Sleep(int ms, SafeCancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(SafeCancellationToken.None))
            {
                Thread.Sleep(ms);
            }
            else
            {
                for (int i = 0; i <= ms && !cancelToken.IsCancellationRequested; i += SleepCancelTokenIterationInterval)
                {
                    Thread.Sleep(SleepCancelTokenIterationInterval);
                }
            }
        }

        public SafeCancellationToken StartTimer(Action<SafeCancellationToken> tick, Func<int> needInterval, bool startImmidiate = true, bool ticksSuperposition = false)
        {
            bool canceled = false;
            bool executionNow = false;
            Timer timer = null;

            var cancellationToken = new SafeCancellationToken();
            cancellationToken.RegisterCallback(() =>
            {
                if (!canceled)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    canceled = true;
                }
            });

            var interval = needInterval?.Invoke() ?? 1000;

            timer = new Timer(
                (t) =>
                {
                    if ((!executionNow || ticksSuperposition) && !cancellationToken.IsCancellationRequested)
                    {
                        executionNow = true;
                        try
                        {
                            tick?.Invoke(cancellationToken);
                        }
                        finally
                        {
                            if (!canceled)
                            {
                                interval = needInterval?.Invoke() ?? 1000;
                                timer?.Change(interval, interval);
                            }
                            executionNow = false;
                        }
                    }
                },
                null,
                startImmidiate ? 0 : interval,
                interval
            );

            return cancellationToken;
        }
    }
}