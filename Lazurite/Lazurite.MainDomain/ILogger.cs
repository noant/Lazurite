using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public abstract class LoggerBase
    {
        public void Write(Exception e, string message = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var str = string.Format("{0} --- {1} --- member: {2} --- line: {3}\r\n--- {4} --- {5}",
                DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"),
                sourceFilePath,
                memberName,
                sourceLineNumber,
                message,
                e.ToString());
            WriteInternal(str);
        }

        protected abstract void WriteInternal(string line);
    }
}
