using Pyrite.MainDomain;
using Pyrite.Windows.ServiceClient;
using Pyrite.Windows.ServiceClient.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.ServiceClient
{
    public class ServiceClientFactory : IClientFactory
    {
        static ServiceClientFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        }

        private Dictionary<ConnectionCredentials, ServerClient> _cache = new Dictionary<ConnectionCredentials, ServerClient>();

        public MainDomain.IServer GetServer(string host, string userLogin, string password)
        {
            var credentials = new ConnectionCredentials() {
                Host = host,
                Login = userLogin,
                Password = password
            };
            if (!_cache.ContainsKey(credentials))
            {
                var sameLoginAndHostItem = _cache.SingleOrDefault(x => x.Key.Host.Equals(host) && x.Key.Login.Equals(userLogin));
                if (sameLoginAndHostItem.Value != null)
                    _cache.Remove(sameLoginAndHostItem.Key);

                _cache.Add(credentials, CreateClient(host, userLogin, password));
            }

            if (_cache[credentials].State == CommunicationState.Faulted ||
                _cache[credentials].State == CommunicationState.Closed ||
                _cache[credentials].State == CommunicationState.Closing)
                _cache[credentials] = CreateClient(host, userLogin, password);

            return _cache[credentials];
        }

        private ServerClient CreateClient(string host, string userLogin, string password)
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            var endpoint = new EndpointAddress(new Uri(host));

            var client = new ServerClient(binding, endpoint);

            client.ClientCredentials.UserName.UserName = userLogin;
            client.ClientCredentials.UserName.Password = password;

            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            client.ChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            return client;
        }
    }
}
