using System;
using System.Security.Cryptography;
using System.Text;

namespace Lazurite.Windows.Utils
{
    public static class CryptoUtils
    {
        public static string CreatePasswordHash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var hashData = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashData);
            }
        }
    }
}
