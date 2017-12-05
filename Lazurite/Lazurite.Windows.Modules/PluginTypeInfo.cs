using System;
using System.Reflection;

namespace Lazurite.Windows.Modules
{
    public class PluginTypeInfo
    {
        public PluginInfo Plugin { get; set; }
        public Type Type { get; set; }
        public Assembly Assembly { get; set; }
    }
}
