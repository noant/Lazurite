using Lazurite.Shared;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lazurite.Data
{
    public abstract class DataEncryptorBase
    {
        public bool Required(Type objectType)
        {
            return objectType.GetCustomAttributes(true).Any(x => x is EncryptFileAttribute);
        }
        
        public abstract bool IsSecretKeyExist { get; }
        public abstract string SecretKey { get; set; }
        public abstract byte[] IV { get; }

        public event EventsHandler<DataEncryptorBase> SecretKeyChanged;

        protected void RaiseSecretKeyChanged()
        {
            SecretKeyChanged?.Invoke(this, new EventsArgs<DataEncryptorBase>(this));
        }

        public byte[] Encrypt(byte[] data)
        {
            return EncryptBytesInternal(data, SecretKey);
        }
        
        public byte[] Decrypt(byte[] encryptedData)
        {
            return DecryptBytesInternal(encryptedData, SecretKey);
        }
        
        // Uses only for local file decryption
        protected virtual byte[] DecryptBytesInternal(byte[] data, string secretKey)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(secretKey);
                aes.IV = IV;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(data))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var msReader = new MemoryStream())
                {
                    cs.CopyTo(msReader);
                    return msReader.ToArray();
                }
            }
        }
        
        // Uses only for local file encryption
        protected virtual byte[] EncryptBytesInternal(byte[] data, string secretKey)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(secretKey);
                aes.IV = IV;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        cs.Write(data, 0, data.Length);
                    return ms.ToArray();
                }
            }
        }
    }
}