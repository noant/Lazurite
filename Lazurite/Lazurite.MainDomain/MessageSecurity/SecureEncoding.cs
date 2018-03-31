using Lazurite.IOC;
using Lazurite.Logging;
using PCLCrypto;
using System;
using System.Linq;
using System.Text;
using static PCLCrypto.WinRTCrypto;

namespace Lazurite.MainDomain.MessageSecurity
{
    public class SecureEncoding
    {
        public readonly static Encoding TextEncoding = Encoding.UTF8;
        private readonly static ILogger Log = Singleton.Resolve<ILogger>();
        private readonly static ISystemUtils Utils = Singleton.Resolve<ISystemUtils>();
        private readonly static Random Rand = new Random();

        public static byte[] CreateIV(string salt, string secretKey)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var secretKeyHashBytes = Utils.CreateMD5Hash(TextEncoding.GetBytes(secretKey));
            //calculate offset of secretKeyHashBytes by summ of 
            //first secretKeyByte and first salt byte in proportion to 256
            var offset = (int)(((secretKeyHashBytes[0] + saltBytes[0]) / 256d) * 16);
            if (offset > 16)
                offset -= 16;
            var targetSecretKeyBytes = new byte[8];
            for (var i = 0; i < targetSecretKeyBytes.Length; i++)
            {
                var index = offset + i;
                if (index >= 8)
                    index -= 8;
                targetSecretKeyBytes[i] = secretKeyHashBytes[index];
            }
            var buff = new byte[16];
            var odd = secretKeyHashBytes[0] > 123; //half byte
            for (var i = 0; i < buff.Length; i++)
            {
                buff[i] = odd ? targetSecretKeyBytes[i/2] : saltBytes[i/2];
                odd = !odd;
            }
            
            return buff;
        }

        public static string CreateSalt()
        {
            var buff = new byte[8];
            Rand.NextBytes(buff);
            return Convert.ToBase64String(buff);
        }

        private string _secretKey;
        
        public SecureEncoding(string secretKey)
        {
            _secretKey = secretKey;
        }

        public SecureEncoding() : this("secret))01234567") { }
        
        public string Encrypt(string data, byte[] iv)
        {
            return Encrypt(TextEncoding.GetBytes(data), iv);
        }

        public string Encrypt(byte[] data, byte[] iv)
        {
            Log.Debug("[SecureEncoding] data encryption begin...");
            try
            {
                return EncryptInternal(data, iv);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Log.Debug("[SecureEncoding] data decryption end...");
            }
        }

        private string EncryptInternal(byte[] data, byte[] iv)
        {
            var algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var key = algo.CreateSymmetricKey(TextEncoding.GetBytes(_secretKey));
            return Convert.ToBase64String(CryptographicEngine.Encrypt(key, data, iv));
        }

        public string Decrypt(string data, byte[] iv)
        {
            var bytes = DecryptBytes(data, iv);
            return TextEncoding.GetString(bytes, 0, bytes.Length);
        }

        private byte[] DecryptBytesInternal(string data, byte[] iv)
        {
            var algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var key = algo.CreateSymmetricKey(TextEncoding.GetBytes(_secretKey));
            return CryptographicEngine.Decrypt(key, Convert.FromBase64String(data), iv);
        }

        public byte[] DecryptBytes(string data, byte[] iv)
        {
            Log.Debug("[SecureEncoding] data decryption begin...");
            try
            {
                return DecryptBytesInternal(data, iv);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Log.Debug("[SecureEncoding] data decryption end...");
            }
        }
    }
}