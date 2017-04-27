using Lazurite.Data;
using Lazurite.Exceptions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios;
using Lazurite.Security;
using Lazurite.Visual;
using Lazurite.Windows.Modules;
using Lazurite.Windows.Server;
using Lazurite.Windows.ServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Core
{
    public class LazuriteCore: IDisposable
    {
        private static readonly string SettingsKey = "coreSettings";

        private CoreSettings _settings;

        public ISavior Savior { get; private set; }
        public IExceptionsHandler ExceptionsHandler { get; private set; }
        public IClientFactory ClientsFactory { get; private set; }
        public PluginsManager PluginsManager { get; private set; }
        public ScenariosRepositoryBase ScenariosRepository { get; private set; }
        public UsersRepositoryBase UsersRepository { get; private set; }
        public VisualSettingsRepository VisualSettingsRepository { get; private set; }
        public LazuriteServer Server { get; private set; }
        
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
            Task.Factory.StartNew(() =>
            {
                Singleton.Add(ExceptionsHandler = new ExceptionsHandler());
                Singleton.Add(Savior = new FileSavior());
                Singleton.Add(ScenariosRepository = new ScenariosRepository());
                Singleton.Add(ClientsFactory = new ServiceClientFactory());
                Singleton.Add(UsersRepository = new UsersRepository());
                Singleton.Add(VisualSettingsRepository = new VisualSettingsRepository());
                PluginsManager = new PluginsManager();
                Server = new LazuriteServer();
                SetSettings(Savior.Get<CoreSettings>(SettingsKey));
                Server.Start();
                callback?.Invoke();
            });
        }
    }
}
