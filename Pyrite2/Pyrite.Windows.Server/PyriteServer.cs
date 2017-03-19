using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.Windows.Service;
using Pyrite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public class PyriteServer
    {
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private ServerSettings _settings;
        private string _key = "serverSettings";
        private ServiceHost _host;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public ServerSettings GetSettings()
        {
            return _settings;
        }

        public void SetSettings(ServerSettings settings)
        {
            _settings = settings;
            _savior.Set(_key, settings);
        }

        public void Stop()
        {
            if (_host != null)
                _host.Close();

            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                var address = new Uri(_settings.GetAddress());
                var service = new PyriteService();
                _host = new WebServiceHost(service, address);
                _host.AddServiceEndpoint(typeof(IServer), binding, address);
                _host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                _host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new LoginValidator();                
                _host.Credentials.ServiceCertificate.SetCertificate(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectName,
                    _settings.CertificateSubject);
                _host.Open();
            },
            _tokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public PyriteServer()
        {
            if (_savior.Has(_key))
                _settings = _savior.Get<ServerSettings>(_key);
            else _settings = new ServerSettings();
        }
    }
}
