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
        public static string GetAssemblyPath(Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            return Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
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
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd()+"\r\n"+process.StandardError.ReadToEnd();
        }
    }
}
