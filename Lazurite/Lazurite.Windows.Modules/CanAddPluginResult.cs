using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Modules
{
    public class CanAddPluginResult
    {
        public CanAddPluginResult(bool can, string message = null, int targetTypesCount=0)
        {
            CanAdd = can;
            Message = message;
            TargetTypesCount = targetTypesCount;
        }

        public bool CanAdd { get; private set; }
        public string Message { get; private set; }
        public int TargetTypesCount { get; private set; }
    }
}
