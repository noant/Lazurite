using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.Scenarios;
using Pyrite.Security;
using Pyrite.Visual;
using Pyrite.Windows.Modules;
using Pyrite.Windows.Server;
using Pyrite.Windows.ServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Core
{
    public class PyriteCore: IDisposable
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
        public PyriteServer Server { get; private set; }
        
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
                Server = new PyriteServer();
                SetSettings(Savior.Get<CoreSettings>(SettingsKey));
                Server.Start();
                callback?.Invoke();
            });
        }
    }
}
