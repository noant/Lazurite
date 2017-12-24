using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace LazuriteUI.Windows.Preparator
{
    public static class Utils
    {
        public static readonly string LazuriteProcessName = "LazuriteUI.Windows.Main";
        public static readonly string VcRedistx64_Path = Path.Combine("ToInstall","vc_redist.x64.exe");
        public static readonly string VcRedistx86_Path = Path.Combine("ToInstall","vc_redist.x86.exe");

        public static void KillAllLazuriteProcesses()
        {
            foreach (var process in Process.GetProcessesByName(LazuriteProcessName))
                process.Kill();
        }

        public static void Install_VC_Redist()
        {
            var log = Singleton.Resolve<ILogger>();

            //x86 installation
            try
            {
                var basePath = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(Utils).Assembly);
                var vcredist = VcRedistx86_Path;
                var path = Path.Combine(basePath, vcredist);
                var result = Lazurite.Windows.Utils.Utils.ExecuteProcess(path, "/install /quiet /norestart /log vcredist_installation_log.txt", false, true);
                log.Info("VcRedist x86 installed with result: [" + result + "]");
            }
            catch (Exception e)
            {
                log.Info("VcRedist x86 installation error", e);
            }

            //x64 installation
            try
            {
                var basePath = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(Utils).Assembly);
                var vcredist = VcRedistx64_Path;
                var path = Path.Combine(basePath, vcredist);
                var result = Lazurite.Windows.Utils.Utils.ExecuteProcess(path, "/install /quiet /norestart /log vcredist_installation_log.txt", false, true);
                log.Info("VcRedist x64 installed with result: [" + result + "]");
            }
            catch (Exception e)
            {
                log.Info("VcRedist x64 installation error", e);
            }
        }
    }
}