using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.Windows.Logging
{
    public class WarningEventArgs : EventArgs
    {
        public WarnType Type { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public WarningEventArgs(WarnType type, string message = null, Exception exception = null)
        {
            Type = type;
            Message = message;
            Exception = exception;
        }
    }
}
