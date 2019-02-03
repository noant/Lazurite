using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.IO;

namespace Lazurite.Data
{
    public abstract class DataManagerBase: IDataManager
    {
        private static readonly DataEncryptorBase Encryptor = Singleton.Resolve<DataEncryptorBase>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        public abstract void Write(string key, byte[] data);
        public abstract byte[] Read(string key);
        public abstract byte[] Serialize<T>(T data);
        public abstract T Deserialize<T>(byte[] stream);
        public abstract void Clear(string key);
        public abstract bool Has(string key);

        private void NotifyNeedSecretCode(Type type)
        {
            Log.Warn($"Секретный код для сохранения файлов не задан, хотя данные типа [{type.Name}] должны сохраняться зашифрованными.");
        }

        public T Get<T>(string key)
        {
            if (Encryptor.Required(typeof(T)))
            {
                if (!Encryptor.IsSecretKeyExist)
                {
                    NotifyNeedSecretCode(typeof(T));
                    Log.Warn($"Попытка загрузить файл [{key}] без расшифровки.");
                    return Deserialize<T>(Read(key));
                }
                else
                {
                    try
                    {
                        var encryptedData = Read(key);
                        return Deserialize<T>(Encryptor.Decrypt(encryptedData));
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Невозможно загрузить зашифрованный файл [{key}]. Попытка загрузить его обычным способом.", e);

                        var loadedNormal = false;

                        byte[] data = null;

                        try
                        {
                            data = Read(key);
                            loadedNormal = true;
                            Log.Warn($"Файл [{key}] был не зашифрован, хотя тип [{typeof(T).Name}] это требует. Он будет пересохранен и зашифрован по секретному ключу.");
                            Write(key, Encryptor.Encrypt(data));
                        }
                        catch (Exception e2)
                        {
                            if (loadedNormal)
                                Log.Error($"Ошибка пересохранения файла [{key}].");
                            else
                                throw e2;
                        }

                        return Deserialize<T>(data);
                    }
                }
            }
            return Deserialize<T>(Read(key));
        }

        public void Set<T>(string key, T data)
        {
            var serialized = Serialize(data);
            var required = Encryptor.Required(data.GetType());
            if (required && !Encryptor.IsSecretKeyExist)
            {
                NotifyNeedSecretCode(typeof(T));
                Log.Warn($"Файл [{key}] будет сохранен обычным способом.");
                Write(key, serialized);
            }
            else if (!required)
                Write(key, serialized);
            else
                Write(key, Encryptor.Encrypt(serialized));
        }
    }
}
