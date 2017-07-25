using HierarchicalData;
using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string ExecuteProcess(string filePath, string arguments)
        {
            var process = new Process();
            process.StartInfo.StandardErrorEncoding =
                process.StartInfo.StandardOutputEncoding =
                Encoding.GetEncoding(866);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            Log.InfoFormat("command executing: [{0} {1}]", filePath, arguments);
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd()+"\r\n"+process.StandardError.ReadToEnd();
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
