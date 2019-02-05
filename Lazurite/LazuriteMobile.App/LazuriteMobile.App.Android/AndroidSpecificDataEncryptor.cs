using System;
using Android.OS;
using Android.Security;
using Android.Security.Keystore;
using Java.Math;
using Java.Security;
using Java.Util;
using Javax.Crypto;
using Javax.Security.Auth.X500;
using Lazurite.Data;

namespace LazuriteMobile.App.Droid
{
    public class AndroidSpecificDataEncryptor: DataEncryptor
    {
        private static readonly string ProtectedDataKey = "LazuriteProtectedData";
        private static readonly string AndroidKeyStoreKey = "AndroidKeyStore";

        private KeyStore _keyStore;
        private readonly bool _sdkLessThan23 = Build.VERSION.SdkInt <= BuildVersionCodes.LollipopMr1;
        private readonly string _cipherTransformationMode = "RSA/ECB/PKCS1Padding";

        private void PrepareKeyStore()
        {
            _keyStore = KeyStore.GetInstance(AndroidKeyStoreKey);
            _keyStore.Load(null);

            if (_keyStore.ContainsAlias(ProtectedDataKey))
                _keyStore.GetKey(ProtectedDataKey, null);
            else
            {
                var context = global::Android.App.Application.Context;

                // thanks to https://dzone.com/articles/xamarin-android-asymmetric-encryption-without-any

                var keyGenerator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, AndroidKeyStoreKey);

                if (_sdkLessThan23)
                {
                    var calendar = Calendar.GetInstance(context.Resources.Configuration.Locale);
                    var endDate = Calendar.GetInstance(context.Resources.Configuration.Locale);
                    endDate.Add(CalendarField.Year, 20);

#pragma warning disable 618
                    var builder = new KeyPairGeneratorSpec.Builder(context)
#pragma warning restore 618
                        .SetAlias(ProtectedDataKey)
                        .SetSerialNumber(BigInteger.One)
                        .SetSubject(new X500Principal($"CN={ProtectedDataKey} CA Certificate"))
                        .SetStartDate(calendar.Time)
                        .SetEndDate(endDate.Time)
                        .SetKeySize(2048);

                    keyGenerator.Initialize(builder.Build());
                }
                else
                {
                    var builder = new KeyGenParameterSpec.Builder(ProtectedDataKey, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                        .SetBlockModes(KeyProperties.BlockModeEcb)
                        .SetEncryptionPaddings(KeyProperties.EncryptionPaddingRsaPkcs1)
                        .SetRandomizedEncryptionRequired(false)
                        .SetKeySize(2048);

                    keyGenerator.Initialize(builder.Build());
                }

                keyGenerator.GenerateKeyPair();
            }
        }

        private IKey PublicKey
        {
            get
            {
                if (_keyStore == null)
                    PrepareKeyStore();
                return _keyStore.GetCertificate(ProtectedDataKey)?.PublicKey;
            }
        }

        private IKey PrivateKey
        {
            get
            {
                if (_keyStore == null)
                    PrepareKeyStore();
                return _keyStore.GetKey(ProtectedDataKey, null);
            }
        }

        public override bool IsSecretKeyExist
        {
            get
            {
                if (!base.IsSecretKeyExist)
                    GenerateRandomSecretKey();
                return true;
            }
        }

        private void GenerateRandomSecretKey()
        {
            // Маловероятно, что пользователь будет переносить данные о подключении
            // из одного телефона на другой, поэтому можно пренебречь тем
            // что при переносе нельзя будет расшифровать сохраненные данные.
            // Поэтому перенос данных с телефона на телефон будет невозможен.
            // Секретный ключ для сохранения данных будет генерироваться динамически,
            // если его не существует.

            var randomizer = new System.Random();
            var alphabetChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[16];
            for (var i = 0; i < 16; i++)
                chars[i] = alphabetChars[randomizer.Next(0, alphabetChars.Length - 1)];
            SecretKey = new string(chars);
        }

        protected override byte[] ProtectData(byte[] data)
        {
#if DEBUG
            try
            {
#endif
                var cipher = Cipher.GetInstance(_cipherTransformationMode);
                cipher.Init(CipherMode.EncryptMode, PublicKey);
                var encryptedData = cipher.DoFinal(data);
                return encryptedData;
#if DEBUG
            }
            catch (Exception e)
            {
                throw e;
            }
#endif
        }

        protected override byte[] UnprotectData(byte[] data)
        {
#if DEBUG
            try
            {
#endif
                var cipher = Cipher.GetInstance(_cipherTransformationMode);
                cipher.Init(CipherMode.DecryptMode, PrivateKey);
                var decryptedData = cipher.DoFinal(data);
                return decryptedData;
#if DEBUG
            }
            catch (Exception e)
            {
                throw e;
            }
#endif
        }
    }
}