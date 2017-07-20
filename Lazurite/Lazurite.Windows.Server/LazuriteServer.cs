using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Service;
using Lazurite.Windows.Utils;
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

namespace Lazurite.Windows.Server
{
    public class LazuriteServer
    {
        private static readonly string StandartSecretKey = "secretKey";
        private static readonly string StandartServiceName = "Lazurite";
        private static readonly ushort StandartServicePort = 8080;
        private static readonly string SettingsKey = "serverSettings";

        private ISavior _savior = Singleton.Resolve<ISavior>();
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private ServerSettings _settings;
        private ServiceHost _host;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        public ServerSettings GetSettings()
        {
            return _settings;
        }

        public void SetSettings(ServerSettings settings)
        {
            _settings = settings;
            _savior.Set(SettingsKey, settings);
        }

        public void Stop()
        {
            if (_host != null)
                _host.Close();

            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        public void StartAsync(Action<bool> callback)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _warningHandler.Info("Service starting: " + this._settings.GetAddress());
                    var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    binding.CloseTimeout =
                        binding.OpenTimeout =
                        binding.SendTimeout = TimeSpan.FromMinutes(5);

                    var address = new Uri(_settings.GetAddress());
                    var service = new LazuriteService(_settings.SecretKey);
                    _host = new WebServiceHost(service, address);
                    _host.AddServiceEndpoint(typeof(IServer), binding, address);
                    _host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                    _host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new LoginValidator();
                    _host.Credentials.ServiceCertificate.SetCertificate(
                        StoreLocation.LocalMachine,
                        StoreName.My,
                        X509FindType.FindByThumbprint,
                        _settings.CertificateHash);
                    _host.Open();
                    _warningHandler.Info("Service started: " + this._settings.GetAddress());
                    callback?.Invoke(true);
                }
                catch (Exception e)
                {
                    _warningHandler.Error("Error while starting service: " + this._settings.GetAddress(), e);
                    callback?.Invoke(false);
                }
            },
            _tokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        public void Restart(Action<bool> callback)
        {
            Stop();
            StartAsync(callback);
        }

        public LazuriteServer()
        {
            if (_savior.Has(SettingsKey))
                _settings = _savior.Get<ServerSettings>(SettingsKey);
            else
            {
                _settings = new ServerSettings() {
                    Port = StandartServicePort,
                    SecretKey = StandartSecretKey,
                    ServiceName = StandartServiceName
                };
            }
        }
    }
}
