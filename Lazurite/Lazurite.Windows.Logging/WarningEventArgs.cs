using Lazurite.Shared;
using System;

namespace Lazurite.Windows.Logging
{
    public class WarningEventArgs : EventsArgs<WarnType>
    {
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public WarningEventArgs(WarnType type, string message = null, Exception exception = null):
            base(type)
        {
            Message = message;
            Exception = exception;
        }
    }
}
