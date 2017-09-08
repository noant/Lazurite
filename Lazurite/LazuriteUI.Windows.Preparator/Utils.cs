using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Preparator
{
    public static class Utils
    {
        public static readonly string LazuriteProcessName = "LazuriteUI.Windows.Main";

        public static void KillAllLazuriteProcesses()
        {
            foreach (var process in Process.GetProcessesByName(LazuriteProcessName))
                process.Kill();
        }
    }
}
