﻿using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Logging
{
    public class Log : LoggerBase
    {
        private static string LocalPath = Lazurite.Windows.Utils.Utils.GetAssemblyPath(Assembly.GetExecutingAssembly());
        protected override void WriteInternal(string line)
        {
            lock (LocalPath)
            {
                File.AppendText(LocalPath).WriteLine(line);
            }
        }
    }
}