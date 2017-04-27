using Lazurite.MainDomain;
using Lazurite.Windows.ServiceClient;
using Lazurite.Windows.ServiceClient.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.ServiceClient
{
    public class ServiceClientFactory : IClientFactory
    {
        static ServiceClientFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        }

        private Dictionary<ConnectionCredentials, ServerClient> _cache = new Dictionary<ConnectionCredentials, ServerClient>();

        public MainDomain.IServer GetServer(string host, ushort port, string serviceName, string secretKey, string userLogin, string password)
        {
            var credentials = new ConnectionCredentials() {
                Host = host,
                Login = userLogin,
                Password = password,
                Port = port,
                ServiceName = serviceName,
                SecretKey = secretKey
            };
            if (!_cache.ContainsKey(credentials))
            {
                _cache.Add(credentials, CreateClient(credentials));
            }

            if (_cache[credentials].State == CommunicationState.Faulted ||
                _cache[credentials].State == CommunicationState.Closed ||
                _cache[credentials].State == CommunicationState.Closing)
                _cache[credentials] = CreateClient(credentials);

            return _cache[credentials];
        }

        private ServerClient CreateClient(ConnectionCredentials credentials)
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxDepth = 50;
            binding.ReaderQuotas.MaxNameTableCharCount = 9999;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;

            binding.CloseTimeout =
                binding.OpenTimeout =
                binding.SendTimeout = TimeSpan.FromMinutes(5);

            var endpoint = new EndpointAddress(new Uri(credentials.GetAddress()));

            var client = new ServerClient(binding, endpoint);

            client.ClientCredentials.UserName.UserName = credentials.Login;
            client.ClientCredentials.UserName.Password = credentials.Password;

            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            client.ChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            
            return client;
        }
    }
}
