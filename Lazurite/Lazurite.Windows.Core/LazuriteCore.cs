using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Utils;
using Lazurite.Visual;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Modules;
using Lazurite.Windows.Server;
using Lazurite.Windows.ServiceClient;
using Lazurite.Windows.Utils;
using System;

namespace Lazurite.Windows.Core
{
    public class LazuriteCore: IDisposable
    {
        private static readonly string SettingsKey = "coreSettings";

        private CoreSettings _settings;

        public ISystemUtils SystemUtils { get; private set; }
        public SaviorBase Savior { get; private set; }
        public IClientFactory ClientsFactory { get; private set; }
        public WarningHandlerBase WarningHandler { get; private set; }
        public PluginsManager PluginsManager { get; private set; }
        public ScenariosRepositoryBase ScenariosRepository { get; private set; }
        public UsersRepositoryBase UsersRepository { get; private set; }
        public VisualSettingsRepository VisualSettingsRepository { get; private set; }
        public LazuriteServer Server { get; private set; }

        public LazuriteCore()
        {
            Singleton.Add(Savior = new FileSavior());
            Singleton.Add(WarningHandler = new WarningHandler());
            Singleton.Add(SystemUtils = new SystemUtils());
        }

        public CoreSettings GetSettings()
        {
            return _settings;
        }

        public void SetSettings(CoreSettings settings)
        {
            _settings = settings;
        }

        public void Dispose()
        {
            ScenariosRepository?.Dispose();
            Server.Stop();
        }

        public void InitializeAsync(Action callback)
        {
            TaskUtils.Start(() =>
            {
                Initialize();
                callback?.Invoke();
            });
        }

        public void Initialize()
        {
            Singleton.Add(ClientsFactory = new ServiceClientFactory());
            Singleton.Add(UsersRepository = new UsersRepository());
            UsersRepository.Initialize();
            Singleton.Add(ScenariosRepository = new ScenariosRepository());
            Singleton.Add(new PluginsDataManager());
            Singleton.Add(PluginsManager = new PluginsManager());
            ScenariosRepository.Initialize();
            Singleton.Add(VisualSettingsRepository = new VisualSettingsRepository());
            Singleton.Add(Server = new LazuriteServer());
            if (Savior.Has(SettingsKey))
                SetSettings(Savior.Get<CoreSettings>(SettingsKey));
            else SetSettings(new CoreSettings());
        }
    }
}
