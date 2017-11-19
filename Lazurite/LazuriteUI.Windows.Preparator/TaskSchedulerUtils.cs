using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Preparator
{
    public static class TaskSchedulerUtils
    {
        public static readonly string TaskFolder = "Lazurite";
        public static readonly string TaskName = "Execute";

        public static void CreateLogonTask(string asUser, string exePath)
        {
            using (var ts = new TaskService())
            {
                var task = ts.NewTask();
                task.Principal.RunLevel = TaskRunLevel.Highest;

                task.Principal.UserId = asUser;

                task.Triggers.Add(new LogonTrigger()
                {
                    Enabled = true,
                    StartBoundary = DateTime.Now,
                    Delay = new TimeSpan(0),
                    UserId = asUser
                });

                task.Triggers.Add(new RegistrationTrigger());

                task.Actions.Add(new ExecAction(exePath, "-FromTaskManager"));
                task.Settings.Enabled = true;
                task.Settings.Hidden = false;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.DisallowStartOnRemoteAppSession = false;
                task.Settings.StopIfGoingOnBatteries = false;
                task.Settings.AllowDemandStart = true;
                task.Settings.StartWhenAvailable = true;
                task.Settings.WakeToRun = true;
                task.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                task.Settings.RestartCount = 5;
                task.Settings.RestartInterval = TimeSpan.FromSeconds(100);
                task.Settings.UseUnifiedSchedulingEngine = true;
                task.Settings.Priority = ProcessPriorityClass.High;
                task.RegistrationInfo.Source = exePath;
                task.RegistrationInfo.Description = "Start Lazurite launcher application";
                task.Settings.ExecutionTimeLimit = new TimeSpan(0);

                var path = Path.Combine(TaskFolder, TaskName);

                ts.RootFolder.RegisterTaskDefinition(path, task, TaskCreation.CreateOrUpdate, asUser, logonType: TaskLogonType.InteractiveToken);
            }
        }

        public static void RemoveLogonTask()
        {
            var path = Path.Combine("Lazurite", "Execute");

            using (var ts = new TaskService())
            {
                ts.GetFolder(TaskFolder).DeleteTask(TaskName);
                ts.RootFolder.DeleteFolder(TaskFolder);
            }
        }
    }
}
