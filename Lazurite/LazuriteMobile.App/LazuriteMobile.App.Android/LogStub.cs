using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lazurite.Logging;

namespace LazuriteMobile.App.Droid
{
    public class LogStub : ILogger
    {
        public void Debug(string message = null, Exception exception = null)
        {
            //do nothing
        }

        public void DebugFormat(string message, params object[] @params)
        {
            //do nothing
        }

        public void Error(string message = null, Exception exception = null)
        {
            //do nothing
        }

        public void ErrorFormat(Exception exception, string message, params object[] @params)
        {
            //do nothing
        }

        public void Fatal(string message = null, Exception exception = null)
        {
            //do nothing
        }

        public void FatalFormat(Exception exception, string message = null, params object[] fatalParams)
        {
            //do nothing
        }

        public void Info(string message = null, Exception exception = null)
        {
            //do nothing
        }

        public void InfoFormat(string message, params object[] infoParams)
        {
            //do nothing
        }

        public void InfoFormat(Exception exception, string message, params object[] infoParams)
        {
            //do nothing
        }

        public void Warn(string message = null, Exception exception = null)
        {
            //do nothing
        }

        public void WarnFormat(string message, params object[] warnParams)
        {
            //do nothing
        }

        public void WarnFormat(Exception exception, string message, params object[] warnParams)
        {
            //do nothing
        }
    }
}