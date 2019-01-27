﻿using Lazurite.MainDomain;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    public class SystemUtils : ISystemUtils
    {
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(300);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public byte[] CreateMD5Hash(byte[] bytes) => MD5.Create().ComputeHash(bytes);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(CancellationToken.None))
                Thread.Sleep(ms);
            else for (int i = 0; i <= ms && !cancelToken.IsCancellationRequested; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
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