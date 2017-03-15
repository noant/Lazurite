using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.Windows.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public class PyriteServer
    {
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private ServerSettings _settings;
        private string _key = "serverSettings";
        private ServiceHost _host;

        public ServerSettings GetSettings()
        {
            return _settings;
        }

        public void SetSettings(ServerSettings settings)
        {
            _settings = settings;
            _savior.Set(_key, settings);
            Restart();
        }

        public void Restart()
        {
            if (_host != null)
                _host.Close();

            _host = new ServiceHost(new PyriteService());
            _host.Open();
        }

        public PyriteServer()
        {

        }
    }
}
