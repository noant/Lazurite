namespace Lazurite.Utils
{
    public static class StringUtils
    {
        public static string TruncateStringNoWrap(string str, int maxLen)
        {
            if (str.Length <= maxLen)
            {
                return str;
            }
            else
            {
                return str.Substring(0, maxLen - 2) + "...";
            }
        }
    }
}