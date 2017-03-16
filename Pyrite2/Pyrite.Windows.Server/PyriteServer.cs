using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.Windows.Service;
using Pyrite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public class PyriteServer
    {
        private readonly byte[] DefaultCertificate = new byte[] { };

        private ISavior _savior = Singleton.Resolve<ISavior>();
        private ServerSettings _settings;
        private string _key = "serverSettings";
        private WebServiceHost _host;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

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

            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                var address = _settings.GetAddress();
                var service = new PyriteService();
                _host = new WebServiceHost(service, new Uri(address));
                _host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                _host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new LoginValidator();

                if (_settings.CertificateLoadMode == CertificateLoadMode.Default)
                    _host.Credentials.ServiceCertificate.Certificate = new X509Certificate2
                    (Path.Combine(Utils.Utils.GetAssemblyPath(Assembly.GetCallingAssembly()), "PyriteStandartCertificate.pfx"), 
                    "1507199215071992");
                else if (_settings.CertificateLoadMode == CertificateLoadMode.SubjectName)
                    _host.Credentials.ServiceCertificate.SetCertificate(_settings.CertificateSubjectName);
                else if (_settings.CertificateLoadMode == CertificateLoadMode.File)
                    _host.Credentials.ServiceCertificate.Certificate = new X509Certificate2(_settings.CertificatePath, _settings.CertificatePassword);

                _host.Open();
            }, 
            _tokenSource.Token);
        }

        public PyriteServer()
        {
            if (_savior.Has(_key))
                _settings = _savior.Get<ServerSettings>(_key);
            else _settings = new ServerSettings();
        }
    }
}
