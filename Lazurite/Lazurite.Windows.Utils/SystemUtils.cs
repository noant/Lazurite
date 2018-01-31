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
            else for (int i = 0; i <= ms && !cancelToken.CanBeCanceled; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }

        public CancellationTokenSource StartTimer(Action<CancellationTokenSource> tick, Func<int> needInterval)
        {
            var cancellationToken = new CancellationTokenSource();

            bool executionNow = false;

            var timer = new System.Timers.Timer();

            timer.Interval = needInterval?.Invoke() ?? 1000;
            timer.Enabled = true;
            timer.AutoReset = true;

            timer.Elapsed += new System.Timers.ElapsedEventHandler((o, e) => {
                if (!executionNow && !cancellationToken.IsCancellationRequested)
                {
                    executionNow = true;
                    try
                    {
                        tick?.Invoke(cancellationToken);
                    }
                    finally
                    {
                        timer.Interval = needInterval?.Invoke() ?? 1000;
                        executionNow = false;
                    }
                }
            });

            cancellationToken.Token.Register(() => {
                timer.Enabled = false;
                timer.Close();
                timer.Dispose();
            });

            timer.Start();

            return cancellationToken;
        }
    }
}
