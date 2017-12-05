using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LazuriteUI.Icons
{
    public static class Utils
    {
        private static Dictionary<Icon, byte[]> AllIconsData = new Dictionary<Icon, byte[]>();

        public static Stream GetIconData(Icon icon)
        {
            if (AllIconsData.ContainsKey(icon))
                return new MemoryStream(AllIconsData[icon]);
            else
            {
                var data = GetIconData(Enum.GetName(typeof(Icon), icon));
                var bytes = ReadFully(data);
                AllIconsData.Add(icon, bytes);
                return GetIconData(icon);
            }
        }
        
        public static Stream GetIconData(string iconName)
        {
            var stream = typeof(Utils)
                    .GetTypeInfo()
                    .Assembly
                    .GetManifestResourceStream(string.Format("LazuriteUI.Icons.Icons.{0}.png", iconName));
            return stream;
        }

        public static byte[] ReadFully(Stream input)
        {
            var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
