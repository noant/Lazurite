using Lazurite.IOC;
using Lazurite.Logging;
using PCLCrypto;
using System;
using System.Text;
using static PCLCrypto.WinRTCrypto;

namespace Lazurite.MainDomain.MessageSecurity
{
    public class SecureEncoding
    {
        private readonly static Encoding Encoding = Encoding.UTF8;
        private readonly static ILogger Log = Singleton.Resolve<ILogger>();
        private readonly static ISystemUtils Utils = Singleton.Resolve<ISystemUtils>();

        private string _secretKey;
        
        public SecureEncoding(string secretKey)
        {
            _secretKey = secretKey;
        }

        public SecureEncoding() : this("secret))01234567") { }
        
        public string Encrypt(string data)
        {
            return Encrypt(Encoding.GetBytes(data));
        }

        public string Encrypt(byte[] data)
        {
            Log.Debug("[SecureEncoding] data encryption begin...");
            try
            {
                return EncryptInternal(data);
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

        private string EncryptInternal(byte[] data)
        {
            var algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var key = algo.CreateSymmetricKey(Encoding.GetBytes(_secretKey));
            return Convert.ToBase64String(CryptographicEngine.Encrypt(key, data));
        }

        public string Decrypt(string data)
        {
            var bytes = DecryptBytes(data);
            return Encoding.GetString(bytes, 0, bytes.Length);
        }

        private byte[] DecryptBytesInternal(string data)
        {
            var algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var key = algo.CreateSymmetricKey(Encoding.GetBytes(_secretKey));
            return CryptographicEngine.Decrypt(key, Convert.FromBase64String(data));
        }

        public byte[] DecryptBytes(string data)
        {
            Log.Debug("[SecureEncoding] data decryption begin...");
            try
            {
                return DecryptBytesInternal(data);
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
