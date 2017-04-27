using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Utils
{
    public static class CryptoUtils
    {
        private static readonly MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
        public static string CreatePasswordHash(string password)
        {
            var hashData = MD5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashData);
        }
    }
}
