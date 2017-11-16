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
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly double ConnectionTimeout_Minutes = GlobalSettings.Get(nameof(ConnectionTimeout_Minutes), 1);

        static ServiceClientFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private Dictionary<ConnectionCredentials, MainDomain.IServer> _cache = new Dictionary<ConnectionCredentials, MainDomain.IServer>();

        public MainDomain.IServer GetServer(ConnectionCredentials credentials)
        {
            MainDomain.IServer @object;

            if (!_cache.ContainsKey(credentials))
                _cache.Add(credentials, @object = CreateProxyClient(credentials));
            else
                @object = _cache[credentials];

            var proxy = (Proxy)@object;
            var connection = (ServerClient)proxy.Obj;

            if (connection.State == CommunicationState.Faulted ||
                connection.State == CommunicationState.Closed ||
                connection.State == CommunicationState.Closing)
                proxy.Obj = CreateClient(credentials);

            return @object;
        }

        private MainDomain.IServer CreateProxyClient(ConnectionCredentials credentials)
        {
            var connection = CreateClient(credentials);
            var connectionProxy = ProxyObject.Create<MainDomain.IServer>(connection, (args) => {
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
                binding.SendTimeout = TimeSpan.FromMinutes(ConnectionTimeout_Minutes);

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
