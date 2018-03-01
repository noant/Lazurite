using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lazurite.Tests
{
    [TestClass]
    public class SecureEncodingTest
    {
        [TestMethod]
        public void SecureEncoding_test1()
        {
            Singleton.Add(new WarningHandler());
            Singleton.Add(new FileSavior());
            Singleton.Add(new SystemUtils());
            
            var str = "test_string test this string string test test one one";
            var secretKey = "0123456789123456";
            var secureEncoding = new SecureEncoding(secretKey);
            for (var i = 0; i <= 1000;i++)
            {
                var salt = SecureEncoding.CreateSalt();
                var iv = SecureEncoding.CreateIV(salt, secretKey);
                var encodedString = secureEncoding.Encrypt(Encoding.UTF8.GetBytes(str), iv);
                iv = SecureEncoding.CreateIV(salt, secretKey);
                var decodedString = secureEncoding.Decrypt(encodedString, iv);
                Debug.WriteLine("encoded = " + encodedString);
                Debug.WriteLine("decoded = " + decodedString);
                Debug.WriteLine("iv = " + Convert.ToBase64String(iv));
                if (str != decodedString)
                {
                    Debug.WriteLine("ERROR!!!!");
                    throw new Exception();
                }
                else Debug.WriteLine("OK");
            }
        }

        [TestMethod]
        public void SecureEncoding_test2()
        {
            Singleton.Add(new WarningHandler());
            Singleton.Add(new FileSavior());
            Singleton.Add(new SystemUtils());

            var str1 = "test_string test this string string test test one one";
            var str2 = "test_string test this3 string str3ing test test one4 one";
            var str3 = "test_string test t123his stri2312ng string 121233test test123 one123 one";
            var str4 = "test_string 1234121";
            var str5 = "test_string tsest thsis strisng stsring tsest stest one sone";
            var str6 = "test_striasdfs";
            var str7 = "test";
            var secretKey = "0123456789123456";
            var secureEncoding = new SecureEncoding(secretKey);
            var salt = SecureEncoding.CreateSalt();
            var iv = SecureEncoding.CreateIV(salt, secretKey);
            Debug.WriteLine(string.Format("encoded 1 = " + secureEncoding.Encrypt(str1, iv)));
            Debug.WriteLine(string.Format("encoded 2 = " + secureEncoding.Encrypt(str2, iv)));
            Debug.WriteLine(string.Format("encoded 3 = " + secureEncoding.Encrypt(str3, iv)));
            Debug.WriteLine(string.Format("encoded 4 = " + secureEncoding.Encrypt(str4, iv)));
            Debug.WriteLine(string.Format("encoded 5 = " + secureEncoding.Encrypt(str5, iv)));
            Debug.WriteLine(string.Format("encoded 6 = " + secureEncoding.Encrypt(str6, iv)));
            Debug.WriteLine(string.Format("encoded 7 = " + secureEncoding.Encrypt(str7, iv)));
        }
    }
}
