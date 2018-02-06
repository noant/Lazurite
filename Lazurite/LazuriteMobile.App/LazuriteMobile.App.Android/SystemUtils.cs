using Lazurite.MainDomain;
using System;
using System.ServiceModel;
using System.Threading;

namespace LazuriteMobile.App.Droid
{
    public class SystemUtils : ISystemUtils
    {
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(300);

        public bool IsFaultException(Exception e) => e is FaultException;

        public bool IsFaultExceptionHasCode(Exception e, string code)
        {
            switch(code)
            {
                case ServiceFaultCodes.AccessDenied:
                    return e.Message.Contains(ServiceFaultCodes.AccessDenied);
                case ServiceFaultCodes.InternalError:
                    return e.Message.Contains(ServiceFaultCodes.InternalError);
            }
            return false;
        }

        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(CancellationToken.None))
                Thread.Sleep(ms);
            else for (int i = 0; i <= ms && !cancelToken.CanBeCanceled; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }

        public CancellationTokenSource StartTimer(Action<CancellationTokenSource> tick, Func<int> needInterval)
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

            timer = new Timer(
                (t) => {
                    if (!executionNow && !cancellationToken.IsCancellationRequested)
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
                                var interval = needInterval?.Invoke() ?? 1000;
                                timer.Change(interval, interval);
                            }
                            executionNow = false;
                        }
                    }
                },
                null,
                0,
                needInterval?.Invoke() ?? 1000
            );

            return cancellationToken;
        }
    }
}