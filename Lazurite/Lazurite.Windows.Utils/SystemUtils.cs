using Lazurite.MainDomain;
using System;
using System.ServiceModel;
using System.Threading;

namespace Lazurite.Windows.Utils
{
    public class SystemUtils : ISystemUtils
    {
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(300);

        public bool IsFaultException(Exception e) => e is FaultException;

        public bool IsFaultExceptionHasCode(Exception e, string code)
        {
            var fault = e as FaultException;
            if (fault != null)
                return fault.Code.Name == code;
            return false;
        }

        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(CancellationToken.None))
                Thread.Sleep(ms);
            else for (int i = 0; i <= ms && !cancelToken.IsCancellationRequested; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }

        public CancellationTokenSource StartTimer(Action<CancellationTokenSource> tick, Func<int> needInterval, bool startImmidiate = true, bool ticksSuperposition = false)
        {
            bool canceled = false;
            bool executionNow = false;
            Timer timer = null;

            var cancellationToken = new CancellationTokenSource();
            cancellationToken.Token.Register(() => {
                if (!canceled)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    canceled = true;
                }
            });

            var interval = needInterval?.Invoke() ?? 1000;

            timer = new Timer(
                (t) => {
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
