using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Icons
{
    public static class Utils
    {
        public static Stream GetIconData(Icon icon)
        {
            return typeof(Utils)
                    .GetTypeInfo()
                    .Assembly
                    .GetManifestResourceStream(string.Format("LazuriteUI.Icons.Icons.{0}.png", Enum.GetName(typeof(Icon), icon)));
        }
    }
}
