using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public static class Utils
    {
        private static int DelayMS = 300;

        /// <summary>
        /// Delay
        /// </summary>
        /// <returns>
        /// The milliseconds sleeps
        /// </returns>
        public static int Sleep()
        {
            Task.Delay(DelayMS).Wait();
            return DelayMS;
        }

        /// <summary>
        /// Delay
        /// </summary>
        /// <returns>
        /// The milliseconds sleeps totally
        /// </returns>
        public static int Sleep(int multiplier, CancellationToken cancelToken)
        {
            var totalSleep = 0;
            for (int i = 0; i < multiplier && !cancelToken.IsCancellationRequested; i++)
                totalSleep += Sleep();
            return totalSleep;
        }
    }
}
