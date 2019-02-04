using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.Data
{
    public abstract class DataManagerBase: IDataManager
    {
        private static readonly DataEncryptorBase Encryptor = Singleton.Resolve<DataEncryptorBase>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly string EncryptedFilesListKey = "encryptedFilesList";

        public abstract void Write(string key, byte[] data);
        public abstract byte[] Read(string key);
        public abstract byte[] Serialize<T>(T data);
        public abstract T Deserialize<T>(byte[] stream);
        public abstract void Clear(string key);
        public abstract bool Has(string key);
        public abstract void Initialize();

        private List<string> _encryptedFiles = new List<string>();

        public DataManagerBase()
        {
            Initialize();
            LoadEncryptedFilesList();
        }

        protected virtual void MarkFileAsEncryted(string key)
        {
            if (!IsFileMarkedAsEncrypted(key))
            {
                _encryptedFiles.Add(key);
                SaveEncryptedFilesList();
            }
        }

        protected virtual void UnmarkFileAsEncrypted(string key)
        {
            if (IsFileMarkedAsEncrypted(key))
            {
                _encryptedFiles.Remove(key);
                SaveEncryptedFilesList();
            }
        }

        protected virtual bool IsFileMarkedAsEncrypted(string key)
        {
            return _encryptedFiles.Contains(key);
        }

        private void LoadEncryptedFilesList()
        {
            if (Has(EncryptedFilesListKey))
            {
                var data = Read(EncryptedFilesListKey);
                var str = Encoding.UTF8.GetString(data);

                _encryptedFiles = 
                    str
                    .Split('\r', '\n')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }
        }

        private void SaveEncryptedFilesList()
        {
            var sb = new StringBuilder();
            foreach (var fileKey in _encryptedFiles)
                sb.AppendLine(fileKey);
            Write(EncryptedFilesListKey, Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public T Get<T>(string key)
        {
            // If program know about type T is required to encryption, then...
            if (Encryptor.Required(typeof(T)))
            {
                // If secret key not exist, program tries to load data without decryption
                if (!Encryptor.IsSecretKeyExist)
                {
                    Log.Warn($"Секретный код для сохранения файлов не задан, хотя данные типа [{typeof(T).Name}] должны сохраняться зашифрованными.");
                    Log.Warn($"Попытка загрузить файл [{key}] без расшифровки.");
                    return Load<T>(key, false);
                }
                // If secret key exist...
                else
                {
                    try
                    {
                        // ...try to load and decrypt with secret code
                        return Load<T>(key, true);
                    }
                    catch (Exception e)
                    {
                        // If try is unsuccessfull, program tries to load data without decryption
                        Log.Error($"Невозможно загрузить зашифрованный файл [{key}]. Попытка загрузить его обычным способом.", e);
                        var loadedNormal = false;
                        T data = default(T);
                        try
                        {
                            // Load data without decryption
                            data = Load<T>(key, false);
                            loadedNormal = true;
                            // If success, then program tries to encrypt and resave data
                            Log.Warn($"Файл [{key}] не был зашифрован, хотя тип [{data.GetType().Name}] это требует. Он будет зашифрован по секретному ключу и пересохранен.");
                            Save(key, data, true);
                        }
                        catch (Exception e2)
                        {
                            if (loadedNormal)
                                Log.Error($"Ошибка пересохранения файла [{key}].", e2);
                            else
                                throw e2;
                        }

                        return data;
                    }
                }
            }

            // If type T is not required to encryption, but file [key] is marked as encrypted
            if (IsFileMarkedAsEncrypted(key))
            {
                // If secret key not exist...
                if (!Encryptor.IsSecretKeyExist)
                {
                    // ...program tries to load data without decryption
                    Log.Warn($"Секретный код для сохранения файлов не задан, хотя файл [{key}] помечен как зашифрованный должны сохраняться зашифрованными.");
                    Log.Warn($"Попытка загрузить файл [{key}] без расшифровки.");
                    return Load<T>(key, false);
                }
                // If secret key exist...
                else
                {
                    try
                    {
                        // ...try to load and decrypt data
                        return Load<T>(key, true);
                    }
                    catch (Exception e)
                    {
                        // If decryption or load try is unsuccessfull, then try to load data without decryption
                        Log.Error($"Невозможно загрузить файл [{key}], который помечен как зашифрованный. Попытка загрузить его обычным способом.", e);
                        return Load<T>(key, false);
                    }
                }
            }
            // If file [key] is not marked as encrypted and type T is not requred to encryption, then...
            else
            {
                // ...load without decryption, and then...
                var data = Load<T>(key, false);
                var type = data.GetType();
                var required = Encryptor.Required(type);

                // ...if loaded data type is required to encryption, program tries to resave data with encryption
                // (loaded data type can be not exactly type T, it can be derived from T and marked with [EncryptFileAttribute])
                if (required && !Encryptor.IsSecretKeyExist)
                {
                    Log.Warn($"Файл [{key}] не был зашифрован, хотя тип [{typeof(T).Name}] это требует. " +
                             "Секретный ключ сохранения файлов не задан, поэтому пересохранить его сейчас в зашифрованном виде невозможно.");
                }
                else if (required)
                {
                    Log.Warn($"Файл [{key}] не был зашифрован, хотя тип [{typeof(T).Name}] это требует. Он будет зашифрован по секретному ключу и пересохранен.");
                    try
                    {
                        Save(key, data, true);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Ошибка пересохранения файла [{key}].", e);
                    }
                }
                return data;
            }
        }

        public void Set<T>(string key, T data)
        {
            var type = data.GetType();
            var required = Encryptor.Required(type);
            if (required && !Encryptor.IsSecretKeyExist)
            {
                Log.Warn($"Файл [{key}] не был зашифрован, хотя тип [{typeof(T).Name}] это требует. Он будет зашифрован по секретному ключу и пересохранен.");
                Log.Warn($"Файл [{key}] будет сохранен обычным способом.");
                Save(key, data, false);
            }
            else if (!required)
                Save(key, data, false);
            else
                Save(key, data, true);
        }

        private T Load<T>(string key, bool encrypted)
        {
            return Deserialize<T>(encrypted ? Encryptor.Decrypt(Read(key)) : Read(key));
        }

        private void Save<T>(string key, T data, bool encrypted)
        {
            if (encrypted)
                MarkFileAsEncryted(key);
            else
                UnmarkFileAsEncrypted(key);

            Write(key, encrypted ? Encryptor.Encrypt(Serialize(data)) : Serialize(data));
        }

    }
}
