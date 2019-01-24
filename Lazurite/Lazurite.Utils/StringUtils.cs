using System;

namespace Lazurite.Utils
{
    public static class StringUtils
    {
        public static string TruncateString(string str, int maxLineWidth)
        {
            if (str.Length <= maxLineWidth)
                return str;

            var parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 1 || parts[0].Length > maxLineWidth)
                return parts[0].Substring(0, maxLineWidth - 2) + "..."; 
            else if (parts.Length == 2)
            {
                if (parts[1].Length > maxLineWidth)
                    return parts[0] + "\r\n" + parts[1].Substring(0, maxLineWidth - 2) + "...";
                else
                    return parts[0] + "\r\n" + parts[1];
            }
            else
            {
                if (parts[1].Length > maxLineWidth - 2)
                    return parts[0] + "\r\n" + parts[1].Substring(0, maxLineWidth - 2) + "...";
                else if (parts[1].Length > maxLineWidth - maxLineWidth / 3)
                    return parts[0] + "\r\n" + parts[1] + "...";
                else
                {
                    var part2 = parts[1] + " " + parts[2];
                    if (part2.Length > maxLineWidth - 2)
                        part2 = part2.Substring(0, maxLineWidth - 2) + "...";
                    return parts[0] + "\r\n" + part2 + (parts.Length > 3 ? "..." : string.Empty);
                }
            }
        }

        public static string TruncateStringNoWrap(string str, int maxLen)
        {
            if (str.Length <= maxLen)
                return str;
            else return str.Substring(0, maxLen - 2) + "...";
        }
    }
}
