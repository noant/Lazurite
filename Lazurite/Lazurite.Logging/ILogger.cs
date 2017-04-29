using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.Logging
{
    public interface ILogger
    {
        void Debug(string message = null, Exception exception = null);

        void Info(string message = null, Exception exception = null);

        void InfoFormat(string message, params object[] infoParams);

        void InfoFormat(Exception exception, string message, params object[] infoParams);

        void Error(string message = null, Exception exception = null);

        void Fatal(string message = null, Exception exception = null);

        void FatalFormat(Exception exception, string message = null, params object[] fatalParams);

        void Warn(string message = null, Exception exception = null);

        void WarnFormat(string message, params object[] warnParams);

        void WarnFormat(Exception exception, string message, params object[] warnParams);

        void DebugFormat(string message, params object[] @params);

        void ErrorFormat(Exception exception, string message, params object[] @params);
    }
}
