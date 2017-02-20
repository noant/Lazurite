using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class CanAddLibraryResult
    {
        public CanAddLibraryResult(bool can, string message = null, int targetTypesCount=0)
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
