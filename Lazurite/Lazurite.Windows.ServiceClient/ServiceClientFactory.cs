using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Utils;
using Lazurite.Windows.ServiceClient;
using Lazurite.Windows.ServiceClient.ServiceReference;
using ProxyObjectCreating;
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
        private static ILogger Log = Singleton.Resolve<ILogger>();

        static ServiceClientFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private Dictionary<ConnectionCredentials, MainDomain.IServer> _cache = new Dictionary<ConnectionCredentials, MainDomain.IServer>();

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
                _cache.Add(credentials, CreateProxyClient(credentials));

            var @object = _cache[credentials];
            var connection = (ServerClient)((Proxy)@object).Obj;

            if (connection.State == CommunicationState.Faulted ||
                connection.State == CommunicationState.Closed ||
                connection.State == CommunicationState.Closing)
                _cache[credentials] = @object = CreateProxyClient(credentials);

            return @object;
        }

        private MainDomain.IServer CreateProxyClient(ConnectionCredentials credentials)
        {
            var connection = CreateClient(credentials);
            var connectionProxy = ProxyObjectCreating.ProxyObject.Create<MainDomain.IServer>(connection, (args) => {
                Log.DebugFormat("Service method entered: [{0}]", args.MethodName);
                var result = args.DefaultReturnValue;
                try
                {
                    result = args.Run();
                }
                catch (Exception e)
                {
                    Log.DebugFormat("Service method error: [{0}]; {1}", args.MethodName, e.InnerException.Message);
                    var targetException = e.InnerException;
                    //if communication exception
                    if (targetException is System.ServiceModel.ServerTooBusyException ||
                                   targetException is System.ServiceModel.ServiceActivationException ||
                                   targetException is System.ServiceModel.AddressAccessDeniedException ||
                                   targetException is System.ServiceModel.AddressAlreadyInUseException ||
                                   targetException is System.ServiceModel.ChannelTerminatedException ||
                                   targetException is System.ServiceModel.CommunicationObjectAbortedException ||
                                   targetException is System.ServiceModel.EndpointNotFoundException ||
                                   targetException is System.ServiceModel.QuotaExceededException ||
                                   targetException is System.ServiceModel.CommunicationObjectFaultedException ||
                                   targetException.GetType().Equals(typeof(CommunicationException)) ||
                                   connection.State == CommunicationState.Closed ||
                                   connection.State == CommunicationState.Closing ||
                                   connection.State == CommunicationState.Faulted)
                        AggregatedCommunicationException.Throw(targetException);
                    else if (targetException is MessageSecurityException && targetException.InnerException != null)
                        throw targetException.InnerException;
                    else
                        throw targetException;
                }
                finally
                {
                    Log.DebugFormat("Service method exit: [{0}]", args.MethodName);
                }
                return result;
            });
            return connectionProxy;
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
