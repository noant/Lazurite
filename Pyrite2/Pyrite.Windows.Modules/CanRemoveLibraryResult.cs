using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class CanRemoveLibraryResult
    {
        public CanRemoveLibraryResult(bool can, string message = null)
        {
            CanRemove = can;
            Message = message;
        }

        public bool CanRemove { get; private set; }
        public string Message { get; private set; }
    }
}
