using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class ModuleAlreadyExistsException : Exception
    {
        public ModuleAlreadyExistsException(string message) : base(message) { } 
    }
}
