using PCLCrypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.Zeros);
            _key = _algo.CreateSymmetricKey(Encoding.UTF8.GetBytes(_secretKey));
        }

        public string Encrypt(string data)
        {
            try
            {
                return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, Encoding.UTF8.GetBytes(data)));
            }
            catch (ObjectDisposedException)
            {
                InitializeAlgorithmProvider();
                return Encrypt(data);
            }
        }

        public string Encrypt(byte[] data)
        {
            try
            {
                return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, data));
            }
            catch (ObjectDisposedException)
            {
                InitializeAlgorithmProvider();
                return Encrypt(data);
            }
        }

        public string Decrypt(string data)
        {
            try
            {
                var bytes = CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            catch (ObjectDisposedException)
            {
                InitializeAlgorithmProvider();
                return Decrypt(data);
            }
        }

        public byte[] DecryptBytes(string data)
        {
            try
            {
                var bytes = CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
                return bytes;
            }
            catch (ObjectDisposedException)
            {
                InitializeAlgorithmProvider();
                return DecryptBytes(data);
            }
        }
    }
}
