using PCLCrypto;
using System;
using System.Text;
using static PCLCrypto.WinRTCrypto;

namespace Lazurite.MainDomain.MessageSecurity
{
    public class SecureEncoding
    {
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
            lock (this)
            {
                _algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.Zeros);
                _key = _algo.CreateSymmetricKey(Encoding.UTF8.GetBytes(_secretKey));
            }
        }

        public string Encrypt(string data)
        {
            return Encrypt(Encoding.UTF8.GetBytes(data));
        }

        public string Encrypt(byte[] data)
        {
            try
            {
                return EncryptInternal(data);
            }
            catch (Exception)
            {
                InitializeAlgorithmProvider();
                return EncryptInternal(data);
            }
        }

        private string EncryptInternal(byte[] data)
        {
            return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, data));
        }

        public string Decrypt(string data)
        {
            var bytes = DecryptBytes(data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private byte[] DecryptBytesInternal(string data)
        {
            return CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
        }

        public byte[] DecryptBytes(string data)
        {
            try
            {
                return DecryptBytesInternal(data);
            }
            catch (Exception)
            {
                InitializeAlgorithmProvider();
                return DecryptBytesInternal(data);
            }
        }
    }
}
