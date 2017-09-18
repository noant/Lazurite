using HierarchicalData;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Utils
{
    public static class Utils
    {
        private static ILogger Log = Singleton.Resolve<ILogger>();

        public static string GetAssemblyPath(Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            return Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
        }

        public static string GetAssemblyFolder(Assembly assembly)
        {
            return Path.GetDirectoryName(GetAssemblyPath(assembly));
        }

        public static string ExecuteProcess(string filePath, string arguments, bool asAdmin=false, bool waitForExit=true)
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
                if (waitForExit)
                {
                    process.WaitForExit();
                    if (!asAdmin)
                        outstr = process.StandardOutput.ReadToEnd() + "\r\n" + process.StandardError.ReadToEnd();
                    else outstr = "command was executed as 'UseShellExecute'";
                }
            }
            catch (Exception e)
            {
                Log.InfoFormat("command executing error: [{0} {1}]", e, e.Message);
                outstr = "command executing error: " + e.Message;
            }
            return outstr;
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        
        public static object CloneObject(object obj)
        {
            var stream = new MemoryStream();
            var hobject = new HObject(stream);
            hobject["clone"] = obj;
            hobject.SaveToStream();
            return HObject.FromStream(stream)["clone"];
        }
    }
}
