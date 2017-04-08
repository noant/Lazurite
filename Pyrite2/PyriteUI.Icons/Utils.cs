using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyriteUI.Icons
{
    public static class Utils
    {
        public static Stream GetIconData(Icon icon)
        {
            return typeof(Utils)
                    .Assembly
                    .GetManifestResourceStream(string.Format("PyriteUI.Icons.Icons.{0}.png", Enum.GetName(typeof(Icon), icon)));
        }
    }
}
