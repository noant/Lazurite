using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Preparator
{
    public static class Utils
    {
        public static readonly string LazuriteProcessName = "LazuriteUI.Windows.Main";
        public static readonly string VcRedistPath_x64 = Path.Combine("ToInstall","vc_redist.x64.exe");
        public static readonly string VcRedistPath_x86 = Path.Combine("ToInstall","vc_redist.x86.exe");

        public static void KillAllLazuriteProcesses()
        {
            foreach (var process in Process.GetProcessesByName(LazuriteProcessName))
                process.Kill();
        }

        public static void Install_VC_Redist()
        {
            var log = Singleton.Resolve<ILogger>();
            try
            {
                var basePath = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(Utils).Assembly);
                var vcredist = Environment.Is64BitOperatingSystem ? VcRedistPath_x64 : VcRedistPath_x86;
                var path = Path.Combine(basePath, vcredist);
                var result = Lazurite.Windows.Utils.Utils.ExecuteProcess(path, "/install /quiet /norestart /log vcredist_installation_log.txt", false, true);
                log.Info("VcRedist installed with result: [" + result + "]");
            }
            catch (Exception e)
            {
                log.Info("VcRedist installation error", e);
            }
        }
    }
}