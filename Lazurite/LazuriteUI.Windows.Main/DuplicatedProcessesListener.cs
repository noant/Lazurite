using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main
{
    public static class DuplicatedProcessesListener
    {
        public static readonly int DuplicatedProcessesListenerInterval = GlobalSettings.Get(1000);
        public static readonly int DuplicatedProcessesListenerInterval_onFound = GlobalSettings.Get(2000);

        public static event Action<Process[]> Found;

        public static void Start()
        {
            var launcherAssembly = typeof(LazuriteUI.Windows.Launcher.App).Assembly;
            var launcherExePath = Path.GetFullPath(Lazurite.Windows.Utils.Utils.GetAssemblyPath(launcherAssembly));
            var launcherProcessName = Path.GetFileNameWithoutExtension(launcherExePath);

            var currentProcess = Process.GetCurrentProcess();
            var currentProcessName = currentProcess.ProcessName;
            var currentProcessLocation = GetProcessFilePath(currentProcess);
            var currentProcessId = currentProcess.Id;
            var action = (Action)(() => {
                while (true)
                {
                    var processes = 
                        Process.GetProcessesByName(currentProcessName)
                        .Union(Process.GetProcessesByName(launcherProcessName))
                        .ToArray();                    
                    var targetProcesses = processes.Where(x =>
                        x.Id != currentProcessId &&
                        ((StringComparer.OrdinalIgnoreCase.Equals(x.ProcessName, launcherProcessName) && 
                        StringComparer.OrdinalIgnoreCase.Equals(GetProcessFilePath(x), launcherExePath)) ||
                        (StringComparer.OrdinalIgnoreCase.Equals(x.ProcessName, currentProcessName) &&
                        StringComparer.OrdinalIgnoreCase.Equals(GetProcessFilePath(x), currentProcessLocation))))
                        .ToArray();
                    if (targetProcesses.Any())
                    {
                        foreach (var process in targetProcesses)
                            process.Kill();
                        Found?.Invoke(targetProcesses);
                        Thread.Sleep(DuplicatedProcessesListenerInterval_onFound);
                    }
                    else
                        Thread.Sleep(DuplicatedProcessesListenerInterval);
                    //crutch
                    foreach (var process in processes)
                        process.Dispose();
                }
            });

            TaskUtils.StartLongRunning(action);
        }

        private static string GetProcessFilePath(Process process)
        {
            if (string.IsNullOrEmpty(process.MainModule.FileName))
                return string.Empty;
            else
                return Path.GetFullPath(process.MainModule.FileName);
        }
    }
}
