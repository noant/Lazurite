using System;
using System.Text;

using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    public static class SingletonPreparator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public static void Initialize()
        {
            if (!Singleton.Any<ILogger>())
                Singleton.Add(new LogStub());
            if (!Singleton.Any<LazuriteContext>())
                Singleton.Add(new LazuriteContext());
            if (!Singleton.Any<DataEncryptorBase>())
                Singleton.Add(new StandardEncryptor());
            if (!Singleton.Any<DataManagerBase>())
                Singleton.Add(new JsonFileManager());
            if (!Singleton.Any<ISystemUtils>())
                Singleton.Add(new SystemUtils());
            if (!Singleton.Any<AddictionalDataManager>())
                Singleton.Add(new AddictionalDataManager());
            if (!Singleton.Any<INotifier>())
                Singleton.Add(new Notifier());
            if (!Singleton.Any<IGeolocationView>())
                Singleton.Add(new GeolocationViewIntentCreator());

            var dataEncryptor = Singleton.Resolve<DataEncryptorBase>();
            if (!dataEncryptor.IsSecretKeyExist)
            {
                // Маловероятно, что пользователь будет переносить данные о подключении
                // из одного телефона на другой, поэтому можно пренебречь тем
                // что при переносе нельзя будет расшифровать сохраненные данные.
                // Поэтому перенос данных с телефона на телефон будет невозможен.
                // Секретный ключ для сохранения данных будет генерироваться динамически,
                // если его не существует.

                var randomizer = new Random();
                var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var keyBuilder = new StringBuilder();
                for (var i = 0; i <= 16; i++)
                    keyBuilder.Append(chars[randomizer.Next(0, chars.Length)]);
                dataEncryptor.SecretKey = keyBuilder.ToString();
            }
        }
    }
}