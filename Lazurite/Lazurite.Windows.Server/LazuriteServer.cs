using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Service;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Threading;

namespace Lazurite.Windows.Server
{
    public class LazuriteServer
    {
        private static readonly int ServerTimoutMinutes = GlobalSettings.Get(1);
        public static readonly string SettingsKey = "serverSettings";
        private SaviorBase _savior = Singleton.Resolve<SaviorBase>();
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private ServerSettings _settings;
        private ServiceHost _host;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        public bool Started { get; private set; }

        public event EventsHandler<LazuriteServer> StatusChanged;

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
            Started = false;
            StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
        }

        public void StartAsync(Action<bool> callback)
        {
            TaskUtils.StartLongRunning(() =>
            {
                _warningHandler.Info("Service starting: " + this._settings.GetAddress());
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.CloseTimeout =
                    binding.OpenTimeout =
                    binding.SendTimeout = TimeSpan.FromMinutes(ServerTimoutMinutes);
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
                Started = true;
                StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            },
            (exception) => {
                _warningHandler.Error("Error while starting service: " + this._settings.GetAddress(), exception);
                callback?.Invoke(false);
                Started = false;
                StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            });
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
                _settings = new ServerSettings();
        }
    }
}
