﻿using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lazurite.Windows.Utils
{
    public class SystemUtils : ISystemUtils
    {
        private static readonly int SleepCancelTokenIterationInterval = GlobalSettings.Get(nameof(SleepCancelTokenIterationInterval), 300);

        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= SleepCancelTokenIterationInterval || cancelToken.Equals(CancellationToken.None))
                Thread.Sleep(ms);
            else for (int i = 0; i <= ms && !cancelToken.CanBeCanceled; i += SleepCancelTokenIterationInterval)
                Thread.Sleep(SleepCancelTokenIterationInterval);
        }
    }
}
