using Lazurite.IOC;
using Lazurite.Logging;
using PCLCrypto;
using System;
using System.Text;
using System.Threading;
using static PCLCrypto.WinRTCrypto;

namespace Lazurite.MainDomain.MessageSecurity
{
    public class SecureEncoding
    {
        private static ILogger Log = Singleton.Resolve<ILogger>();
        private static ISystemUtils Utils = Singleton.Resolve<ISystemUtils>();
        private string _secretKey;
        private ISymmetricKeyAlgorithmProvider _algo;
        private ICryptographicKey _key;
        
        public SecureEncoding(string secretKey)
        {
            _secretKey = secretKey;
            InitializeAlgorithmProvider();
        }

        public SecureEncoding() : this("secret))01234567") { }

        private void InitializeAlgorithmProvider()
        {
            _key = null;
            _algo = null;
            Log.Debug("[SecureEncoding] algorithm provider refreshing...");
            _algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.Zeros);
            _key = _algo.CreateSymmetricKey(Encoding.UTF8.GetBytes(_secretKey));
            Log.Debug("[SecureEncoding] algorithm provider refreshed");
        }

        private void WaitForAlgorithmProviderInitialized()
        {
            while (_key == null && _algo == null)
                Utils.Sleep(10, CancellationToken.None);
        }

        public string Encrypt(string data)
        {
            return Encrypt(Encoding.UTF8.GetBytes(data));
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
                InitializeAlgorithmProvider();
                return EncryptInternal(data);
            }
            finally
            {
                Log.Debug("[SecureEncoding] data decryption end...");
            }
        }

        private string EncryptInternal(byte[] data)
        {
            WaitForAlgorithmProviderInitialized();
            return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, data));
        }

        public string Decrypt(string data)
        {
            var bytes = DecryptBytes(data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private byte[] DecryptBytesInternal(string data)
        {
            WaitForAlgorithmProviderInitialized();
            return CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
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
                InitializeAlgorithmProvider();
                return DecryptBytesInternal(data);
            }
            finally
            {
                Log.Debug("[SecureEncoding] data decryption end...");
            }
        }
    }
}
