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
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Threading;

namespace Lazurite.Windows.Server
{
    public class LazuriteServer
    {
        private static readonly int ServerTimeoutMinutes = GlobalSettings.Get(3);
        private static readonly int MaxConcurrentSessions = GlobalSettings.Get(30);
        private static readonly int MaxConcurrentCalls = GlobalSettings.Get(100);
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
                _warningHandler.Info("Service starting: " + _settings.GetAddress());
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                binding.ReaderQuotas.MaxNameTableCharCount = 9999;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.ReaderQuotas.MaxDepth = 50;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.CloseTimeout =
                    binding.OpenTimeout =
                    binding.SendTimeout = TimeSpan.FromMinutes(ServerTimeoutMinutes);
                var address = new Uri(_settings.GetAddress());
                var service = new LazuriteService(_settings.SecretKey);
                _host = new WebServiceHost(service, address);

                var serviceThrottlingBehavior = new ServiceThrottlingBehavior();
                serviceThrottlingBehavior.MaxConcurrentSessions = MaxConcurrentSessions;
                serviceThrottlingBehavior.MaxConcurrentCalls = MaxConcurrentCalls;
                _host.Description.Behaviors.Add(serviceThrottlingBehavior);

                var debuggingBehavior = _host.Description.Behaviors[typeof(ServiceDebugBehavior)] as ServiceDebugBehavior;
                debuggingBehavior.IncludeExceptionDetailInFaults = true;
                
                _host.AddServiceEndpoint(typeof(IServer), binding, address);
                _host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                _host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new LoginValidator();
                _host.Credentials.ServiceCertificate.SetCertificate(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindByThumbprint,
                    _settings.CertificateHash);
                _host.Open();
                _warningHandler.Info("Service started: " + _settings.GetAddress());
                callback?.Invoke(true);
                Started = true;
                StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            },
            (exception) => {
                _warningHandler.Error("Ошибка запуска сервиса: " + _settings.GetAddress(), exception);
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
