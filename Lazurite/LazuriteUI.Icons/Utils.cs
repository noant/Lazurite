using System;
using System.IO;
using System.Reflection;

namespace LazuriteUI.Icons
{
    public static class Utils
    {
        public static Stream GetIconData(Icon icon)
        {
            return GetIconData(GetIconResourceName(icon));
        }
        
        public static string GetIconResourceName(Icon icon)
        {
            return string.Format("LazuriteUI.Icons.Icons.{0}.png", Enum.GetName(typeof(Icon), icon));
        }

        public static Stream GetIconData(string iconPath)
        {
            var stream = typeof(Utils)
                    .GetTypeInfo()
                    .Assembly
                    .GetManifestResourceStream(iconPath);

            return stream;
        }

        public static byte[] GetIconDataBytes(Icon icon)
        {
            var stream = GetIconData(icon);
            var buff = new byte[stream.Length];
            stream.Read(buff, 0, buff.Length);
            return buff;
        }
    }
}
