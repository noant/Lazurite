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
            var output = "";
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.ErrorDataReceived += (o, e) => output += e.Data + "\r\n";
            process.OutputDataReceived += (o, e) => output += e.Data + "\r\n";
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return output;
        }
    }
}
