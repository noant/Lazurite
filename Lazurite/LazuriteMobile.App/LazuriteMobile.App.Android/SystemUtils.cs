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
    }
}