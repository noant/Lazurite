using Konscious.Security.Cryptography;
using Lazurite.IOC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lazurite.Data
{
    public class StandardEncryptor : DataEncryptorBase
    {
        private static readonly string EncryptedSecretKeyKey = "FileSave_EncryptedSecKey";
        private static readonly string CheckKeyFileKey = "FileSave_EncryptedSecKeyHash";
        private static readonly string IvKey = "FileSave_Iv";

        private string _secretKey = null;
        private byte[] _iv = null;

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

        public override bool IsSecretKeyExist
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

        public override string SecretKey
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

                        // Decrypt secret key with DPAPI
                        var secretKeyBytes = ProtectedData.Unprotect(encryptedSecretKey, null, DataProtectionScope.CurrentUser);
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
                var encryptedSecretKeyBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser);

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

        /// <summary>
        /// Uses only for local file encryption
        /// </summary>
        public override byte[] IV
        {
            get
            {
                if (_iv != null)
                    return _iv;

                byte[] generateIv() {
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
    }
}
