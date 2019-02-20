using HierarchicalData;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace Lazurite.Windows.Utils
{
    public static class Utils
    {
        private static ILogger Log = Singleton.Resolve<ILogger>();

        public static string GetAssemblyPath(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
        }

        public static string GetAssemblyFolder(Assembly assembly) => Path.GetDirectoryName(GetAssemblyPath(assembly));

        public static string ExecuteProcess(string filePath, string arguments, bool asAdmin = false, bool waitForExit = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;

            var outstr = string.Empty;

            if (asAdmin)
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
            }
            else if (waitForExit)
            {
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.StandardErrorEncoding =
                    process.StartInfo.StandardOutputEncoding =
                    Encoding.GetEncoding(866);
            }

            try
            {
                Log.InfoFormat("command executing: [{0} {1}]", filePath, arguments);
                process.Start();
                process.PriorityClass = priority;
                if (waitForExit)
                {
                    process.WaitForExit();
                    if (!asAdmin)
                    {
                        outstr = process.StandardOutput.ReadToEnd() + "\r\n" + process.StandardError.ReadToEnd();
                    }
                    else
                    {
                        outstr = "command was executed as 'UseShellExecute'";
                    }
                }
            }
            catch (Exception e)
            {
                outstr = string.Format("command executing error: [{0} {1}]", e.Message, e);
            }

            if (!string.IsNullOrWhiteSpace(outstr))
            {
                Log.InfoFormat("command [{0} {1}] execution result:\r\n{2}", filePath, arguments, outstr);
            }

            return outstr;
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static object CloneObject(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var hobject = new HObject();
                hobject["clone"] = obj;
                hobject.SaveToStream(stream);
                return HObject.FromStream(stream)["clone"];
            }
        }
    }
}