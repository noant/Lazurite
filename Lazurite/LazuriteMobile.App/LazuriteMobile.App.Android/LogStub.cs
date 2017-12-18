using Lazurite.Logging;
using System;
using System.IO;

namespace LazuriteMobile.App.Droid
{
    public class LogStub : ILogger
    {
        private string _logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "log.txt");
        private object _locker = new object();

        public void Debug(string message = null, Exception exception = null)
        {
            InternalWrite(WriteType.Debug, exception, message);
        }

        public void DebugFormat(string message, params object[] @params)
        {
            InternalWrite(WriteType.Debug, null, string.Format(message, @params));
        }

        public void Error(string message = null, Exception exception = null)
        {
            InternalWrite(WriteType.Error, exception, message);
        }

        public void ErrorFormat(Exception exception, string message, params object[] @params)
        {
            InternalWrite(WriteType.Error, exception, string.Format(message, @params));
        }

        public void Fatal(string message = null, Exception exception = null)
        {
            InternalWrite(WriteType.Fatal, exception, message);
        }

        public void FatalFormat(Exception exception, string message = null, params object[] @params)
        {
            InternalWrite(WriteType.Fatal, exception, string.Format(message, @params));
        }

        public void Info(string message = null, Exception exception = null)
        {
            InternalWrite(WriteType.Info, exception, message);
        }

        public void InfoFormat(string message, params object[] @params)
        {
            InternalWrite(WriteType.Info, null, string.Format(message, @params));
        }

        public void InfoFormat(Exception exception, string message, params object[] @params)
        {
            InternalWrite(WriteType.Info, exception, string.Format(message, @params));
        }

        public void Warn(string message = null, Exception exception = null)
        {
            InternalWrite(WriteType.Warn, exception, message);
        }

        public void WarnFormat(string message, params object[] @params)
        {
            InternalWrite(WriteType.Warn, null, string.Format(message, @params));
        }

        public void WarnFormat(Exception exception, string message, params object[] @params)
        {
            InternalWrite(WriteType.Warn, exception, string.Format(message, @params));
        }

        private enum WriteType
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        private void InternalWrite(WriteType type, Exception exception, string message)
        {
            lock(_locker)
            {
                if (new FileInfo(_logFile).Length > 1024 * 1024 * 1024) //1mb
                    File.WriteAllText(_logFile, PrepareLine(type, exception, message));
                else File.AppendAllLines(_logFile, new[] { PrepareLine(type, exception, message) });
            }
        }

        private string PrepareLine(WriteType type, Exception exception, string message)
        {
            if (exception == null)
                return string.Format("{0} {1} {2} {3} {4}\r\n{5}\r\n",
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToShortTimeString(),
                    Enum.GetName(typeof(WriteType), type),
                    message,
                    exception.Message,
                    exception.StackTrace);
            else
                return string.Format("{0} {1} {2} {3}",
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToShortTimeString(),
                    Enum.GetName(typeof(WriteType), type),
                    message);
        }
    }
}