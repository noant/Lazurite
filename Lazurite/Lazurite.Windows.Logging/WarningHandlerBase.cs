using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.Windows.Logging
{
    public abstract class WarningHandlerBase: ILogger
    {
        private static SaviorBase Savior = Singleton.Resolve<SaviorBase>();

        private WarnType? _maxWritingWarnType = null;
        public WarnType MaxWritingWarnType
        {
            get
            {
                if (_maxWritingWarnType == null)
                {
                    if (Savior.Has(nameof(MaxWritingWarnType)))
                        _maxWritingWarnType = Savior.Get<WarnType>(nameof(MaxWritingWarnType));
                    else
                        MaxWritingWarnType = WarnType.Info;
                }
                return _maxWritingWarnType.Value;
            }
            set
            {
                _maxWritingWarnType = value;
                Savior.Set(nameof(MaxWritingWarnType), _maxWritingWarnType);
                InternalWrite(WarnType.Info, "Выставлен уровень логирования: " + Enum.GetName(typeof(WarnType), _maxWritingWarnType));
            }
        }

        public abstract void InternalWrite(WarnType type, string message = null, Exception exception = null);

        public void Write(WarnType type, string message = null, Exception exception = null)
        {
            if (type == WarnType.Debug)
                System.Diagnostics.Debug.WriteLine(message);
            if (type <= MaxWritingWarnType)
                InternalWrite(type, message, exception);
            RaiseOnWrite(type, message, exception);
        }

        public void Debug(string message = null, Exception exception = null)
        {
            Write(WarnType.Debug, message, exception);
        }

        public void Info(string message = null, Exception exception = null)
        {
            Write(WarnType.Info, message, exception);
        }

        public void InfoFormat(string message, params object[] infoParams)
        {
            Write(WarnType.Info, string.Format(message, infoParams), null);
        }

        public void Error(string message = null, Exception exception = null)
        {
            Write(WarnType.Error, message, exception);
        }

        public void Fatal(string message = null, Exception exception = null)
        {
            Write(WarnType.Fatal, message, exception);
        }

        public void Warn(string message = null, Exception exception = null)
        {
            Write(WarnType.Warn, message, exception);
        }

        public void WarnFormat(string message, params object[] warnParams)
        {
            Write(WarnType.Warn, string.Format(message, warnParams), null);
        }

        public void DebugFormat(string message, params object[] @params)
        {
            Write(WarnType.Debug, string.Format(message, @params), null);
        }

        public void ErrorFormat(Exception exception, string message, params object[] @params)
        {
            Write(WarnType.Error, string.Format(message, @params), exception);
        }

        public void InfoFormat(Exception exception, string message, params object[] infoParams)
        {
            Write(WarnType.Info, string.Format(message, infoParams), exception);
        }

        public void FatalFormat(Exception exception, string message = null, params object[] fatalParams)
        {
            Write(WarnType.Fatal, string.Format(message, fatalParams), exception);
        }

        public void WarnFormat(Exception exception, string message, params object[] warnParams)
        {
            Write(WarnType.Warn, string.Format(message, warnParams), exception);
        }

        private void RaiseOnWrite(WarnType type, string message, Exception exception)
        {
            OnWrite?.Invoke(this, new WarningEventArgs(type, message, exception));
        }

        public event Action<object, WarningEventArgs> OnWrite;
    }
}
