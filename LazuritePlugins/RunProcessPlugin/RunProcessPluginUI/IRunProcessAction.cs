using System.Diagnostics;

namespace RunProcessPluginUI
{
    public interface IRunProcessAction
    {
        string ExePath { get; set; }
        string Arguments { get; set; }
        CloseProcessMode CloseMode { get; set; }
        ProcessPriorityClass Priority { get; set; }
    }
}
