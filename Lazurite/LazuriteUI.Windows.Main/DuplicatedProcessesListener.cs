using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LazuriteUI.Windows.Main
{
    public static class DuplicatedProcessesListener
    {
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static ILogger Log = Singleton.Resolve<ILogger>();

        private static readonly int DuplicatedProcessesListenerInterval = GlobalSettings.Get(1000);
        private static readonly int DuplicatedProcessesListenerInterval_onFound = GlobalSettings.Get(2000);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event EventsHandler<Process[]> Found;

        public static void Start()
        {
            var launcherAssembly = typeof(LazuriteUI.Windows.Launcher.App).Assembly;
            var launcherExePath = Path.GetFullPath(Lazurite.Windows.Utils.Utils.GetAssemblyPath(launcherAssembly));
            var launcherProcessName = Path.GetFileNameWithoutExtension(launcherExePath);

            var currentProcess = Process.GetCurrentProcess();
            var currentProcessName = currentProcess.ProcessName;
            var currentProcessLocation = GetProcessFilePath(currentProcess);
            var currentProcessId = currentProcess.Id;

            var listenInterval = DuplicatedProcessesListenerInterval;

            var action = (Action)(() => {
                try
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
                        Found?.Invoke(App.Current, new EventsArgs<Process[]>(targetProcesses));
                        listenInterval = DuplicatedProcessesListenerInterval_onFound;
                    }
                    else
                        listenInterval = DuplicatedProcessesListenerInterval;
                    //crutch
                    foreach (var process in processes)
                        process.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error("Ошибка во время получения процессов в DuplicatedProcessesListener. Возможной причиной может являться антивирус или отсутвие доступа.", e);
                }
            });

            SystemUtils.StartTimer((token) => action(), () => listenInterval);
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
