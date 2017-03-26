using PCLCrypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PCLCrypto.WinRTCrypto;

namespace Pyrite.MainDomain.MessageSecurity
{
    public class SecureEncoding
    {
        private string _secretKey;
        private ISymmetricKeyAlgorithmProvider _algo = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.Zeros);
        private ICryptographicKey _key;
        
        public SecureEncoding(string secretKey)
        {
            _secretKey = secretKey;
            _key = _algo.CreateSymmetricKey(Encoding.UTF8.GetBytes(_secretKey));
        }

        public SecureEncoding() : this("secret))01234567") { }

        public string Encrypt(string data)
        {
            return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, Encoding.UTF8.GetBytes(data)));
        }

        public string Encrypt(byte[] data)
        {
            return Convert.ToBase64String(CryptographicEngine.Encrypt(_key, data));
        }

        public string Decrypt(string data)
        {
            var bytes = CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public byte[] DecryptBytes(string data)
        {
            var bytes = CryptographicEngine.Decrypt(_key, Convert.FromBase64String(data));
            return bytes;
        }
    }
}
