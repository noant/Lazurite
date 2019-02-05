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
        private static readonly string VcRedistDir = "VcRedist";

        public static void KillAllLazuriteProcesses()
        {
            foreach (var process in Process.GetProcessesByName(LazuriteProcessName))
                process.Kill();
        }

        private static bool VcRedistInstall(string filePath)
        {
            var log = Singleton.Resolve<ILogger>();
            try
            {
                var result = Lazurite.Windows.Utils.Utils.ExecuteProcess(filePath, "/install /quiet /norestart /log vcredist_installation_log.txt", false, true);
                log.InfoFormat("{0} installed with result: [{1}]", filePath, result);
                return true;
            }
            catch (Exception e)
            {
                log.InfoFormat("{0} installation error", e);
                return false;
            }
        }

        public static bool VcRedistInstallAll()
        {
            var log = Singleton.Resolve<ILogger>();
            try
            {
                var basePath = Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(Utils).Assembly);
                var vcRedistDir = Path.Combine(basePath, VcRedistDir);
                var success = true;
                foreach (var filePath in Directory.GetFiles(vcRedistDir))
                    success &= VcRedistInstall(filePath);
                return success;
            }
            catch (Exception e)
            {
                log.Info("VcRedist installation error", e);
                return false;
            }
        }
    }
}