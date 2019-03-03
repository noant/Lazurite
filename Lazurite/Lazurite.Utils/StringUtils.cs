using System.Linq;

namespace Lazurite.Utils
{
    public static class StringUtils
    {
        public static string TruncateStringNoWrap(string str, int maxLen)
        {
            var upperChars = str.Count(x => char.IsUpper(x)) * 1.5; // Upper char factor
            var lowerChars = str.Count(x => char.IsLower(x));

            if (upperChars + lowerChars <= maxLen)
            {
                return str;
            }
            else
            {
                var splitCount = 2 + ((int)upperChars / 5);
                return str.Substring(0, maxLen - splitCount) + "...";
            }
        }
    }
}