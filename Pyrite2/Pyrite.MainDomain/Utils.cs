using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public static class Utils
    {
        private static int DelayMS = 100;

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
        public static int Sleep(int multiplier)
        {
            Task.Delay(DelayMS*multiplier).Wait();
            return DelayMS*multiplier;
        }
    }
}
