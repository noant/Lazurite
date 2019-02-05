using Konscious.Security.Cryptography;
using Lazurite.IOC;
using Lazurite.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lazurite.Data
{
    public class DataEncryptor
    {
        private static readonly string EncryptedSecretKeyKey = "FileSave_EncryptedSecKey";
        private static readonly string CheckKeyFileKey = "FileSave_EncryptedSecKeyHash";
        private static readonly string IvKey = "FileSave_Iv";

        private string _secretKey = null;
        private byte[] _iv = null;

        public bool Required(Type objectType)
        {
            return objectType.GetCustomAttributes(true).Any(x => x is EncryptFileAttribute);
        }
        
        public byte[] Encrypt(byte[] data)
        {
            return EncryptBytesInternal(data, SecretKey);
        }
        
        public byte[] Decrypt(byte[] encryptedData)
        {
            return DecryptBytesInternal(encryptedData, SecretKey);
        }
        
        // Use only for local file decryption
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
        
        // Use only for local file encryption
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

        protected virtual byte[] ProtectData(byte[] data)
        {
            // Encrypt data with DPAPI
            return ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        }

        protected virtual byte[] UnprotectData(byte[] data)
        {
            // Decrypt data with DPAPI
            return ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
        }

        private bool CheckSecretKey(DataManagerBase manager, string secretKey)
        {
            try
            {
                var encryptedData = manager.Read(CheckKeyFileKey);
                var decryptedHash = DecryptBytesInternal(encryptedData, secretKey);
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

                using (var blake2 = new HMACBlake2B(512))
                {
                    var currentHash = blake2.ComputeHash(secretKeyBytes);
                    return Enumerable.SequenceEqual(currentHash, decryptedHash);
                }
            }
#if DEBUG
            catch (Exception e)
            {
                Debug.WriteLine("Error while checking secret key. " + e.Message);
                return false;
            }
#else
            catch
            {
                return false;
            }
#endif
        }

        public virtual bool IsSecretKeyExist
        {
            get
            {
                if (_secretKey != null)
                    return true;

                try
                {
                    var secretKey = SecretKey; // Crutch
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public virtual string SecretKey
        {
            get
            {
                if (_secretKey == null)
                {
                    try
                    {
                        var manager = Singleton.Resolve<DataManagerBase>();

                        if (!manager.Has(EncryptedSecretKeyKey))
                            throw new CryptographicException("Secret key not exist.");

                        var encryptedSecretKey = manager.Read(EncryptedSecretKeyKey);

                        var secretKeyBytes = UnprotectData(encryptedSecretKey);
                        var secretKey = Encoding.UTF8.GetString(secretKeyBytes);

                        if (secretKey.Length != 16)
                            throw new CryptographicException("Secret key not valid.");

                        if (!CheckSecretKey(manager, secretKey))
                            throw new CryptographicException("Secret key not valid.");

                        return _secretKey = secretKey;
                    }
                    catch (Exception e)
                    {
                        throw new CryptographicException("Cannot get secret key.", e);
                    }
                }
                return _secretKey;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length != 16)
                    throw new ArgumentException("Key must be 16 symbols string.");

                _secretKey = value;

                // Encrypt with DPAPI
                var encryptedSecretKeyBytes = ProtectData(Encoding.UTF8.GetBytes(value));

                var manager = Singleton.Resolve<DataManagerBase>();

                // Write encrypted secret key
                manager.Write(EncryptedSecretKeyKey, encryptedSecretKeyBytes);

                // Encrypt hash of secret key and then check it on load
                using (var blake2 = new HMACBlake2B(512))
                {
                    var keyBytes = Encoding.UTF8.GetBytes(value);
                    var keyHash = blake2.ComputeHash(keyBytes);
                    var encrypted = Encrypt(keyHash);
                    manager.Write(CheckKeyFileKey, encrypted);
                }

                RaiseSecretKeyChanged();
            }
        }

        /// Use only for local file encryption
        public virtual byte[] IV
        {
            get
            {
                if (_iv != null)
                    return _iv;

                byte[] generateIv()
                {
                    using (var r = RandomNumberGenerator.Create())
                    {
                        var buff = new byte[16];
                        r.GetBytes(buff);
                        return buff;
                    }
                }

                var manager = Singleton.Resolve<DataManagerBase>();
                if (manager.Has(IvKey))
                {
                    var iv = manager.Read(IvKey);
                    if (iv != null && iv.Length == 16)
                        _iv = iv;
                    else
                        manager.Write(IvKey, _iv = generateIv());
                }
                else
                    manager.Write(IvKey, _iv = generateIv());

                return _iv;
            }
        }

        protected void RaiseSecretKeyChanged()
        {
            SecretKeyChanged?.Invoke(this, new EventsArgs<DataEncryptor>(this));
        }

        public event EventsHandler<DataEncryptor> SecretKeyChanged;
    }
}