using Lazurite.MainDomain;
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
        private static int Iteration = 300;

        public void Sleep(int ms, CancellationToken cancelToken)
        {
            if (ms <= Iteration)
                Thread.Sleep(ms);
            for (int i = 0; i <= ms && !cancelToken.CanBeCanceled; i += Iteration)
                Thread.Sleep(Iteration);
        }
    }
}
