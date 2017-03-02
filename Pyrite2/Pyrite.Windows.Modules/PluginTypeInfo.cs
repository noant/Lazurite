using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class PluginTypeInfo
    {
        public PluginInfo Plugin { get; set; }
        public Type Type { get; set; }
        public Assembly Assembly { get; set; }
    }
}
