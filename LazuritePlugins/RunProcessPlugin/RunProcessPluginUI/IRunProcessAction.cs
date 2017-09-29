using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
