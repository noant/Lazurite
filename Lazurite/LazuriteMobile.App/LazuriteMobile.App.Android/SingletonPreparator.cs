using System;
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
            if (!Singleton.Any<DataEncryptor>())
                Singleton.Add(new AndroidSpecificDataEncryptor());
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
        }
    }
}