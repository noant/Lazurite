using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using ProxyObjectCreating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace Lazurite.Windows.ServiceClient
{
    public class ServiceClientFactory : IClientFactory
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly double ConnectionTimeout_Minutes = GlobalSettings.Get(1.0d);
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        static ServiceClientFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private Dictionary<ConnectionCredentials, IServer> _cache = new Dictionary<ConnectionCredentials, IServer>();

        public ConnectionCredentials[] ConnectionCredentials => _cache.Keys.ToArray();

        public MainDomain.IServer GetServer(ConnectionCredentials credentials)
        {
            IServer @object;

            lock (_cache)
            {
                if (!_cache.ContainsKey(credentials))
                    _cache.Add(credentials, @object = CreateProxyClient(credentials));
                else
                    @object = _cache[credentials];
            }

            var proxy = (Proxy)@object;
            var connection = (ICommunicationObject)proxy.Obj;
            if (connection.State == CommunicationState.Faulted ||
                connection.State == CommunicationState.Closed ||
                connection.State == CommunicationState.Closing)
                proxy.Obj = CreateClient(credentials);
            
            return @object;
        }

        private IServer CreateProxyClient(ConnectionCredentials credentials)
        {
            var channelFactory = CreateClient(credentials);
            var connection = channelFactory.CreateChannel();
            IServer connectionProxy = null;
            var isFailed = false;
            connectionProxy = ProxyObject.Create(connection, (args) => {
                Log.DebugFormat("Service method entered: [{0}]", args.MethodName);
                var result = args.DefaultReturnValue;
                try
                {
                    result = args.Run();
                    if (!isFailed)
                    {
                        isFailed = true;
                        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connectionProxy, isFailed, credentials));
                    }
                }
                catch (Exception e)
                {
                    Log.DebugFormat("Service method error: [{0}]; {1}", args.MethodName, e.InnerException.Message);
                    var targetException = e.InnerException;
                    if (isFailed 
                        && !SystemUtils.IsFaultExceptionHasCode(targetException, ServiceFaultCodes.ObjectNotFound)
                        && !SystemUtils.IsFaultExceptionHasCode(targetException, ServiceFaultCodes.ObjectAccessDenied))
                    {
                        isFailed = false;
                        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connectionProxy, isFailed, credentials));
                    }
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
                        targetException is System.TimeoutException ||
                        channelFactory.State == CommunicationState.Closed ||
                        channelFactory.State == CommunicationState.Closing ||
                        channelFactory.State == CommunicationState.Faulted)
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

        private ChannelFactory<IServer> CreateClient(ConnectionCredentials credentials)
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = 9999;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = 50;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.CloseTimeout =
                binding.ReceiveTimeout =
                binding.OpenTimeout =
                binding.SendTimeout = TimeSpan.FromMinutes(ConnectionTimeout_Minutes);

            var endpoint = new EndpointAddress(new Uri(credentials.GetAddress()));

            var channelFactory = new ChannelFactory<IServer>(binding, endpoint);

            channelFactory.Credentials.UserName.UserName = credentials.Login;
            channelFactory.Credentials.UserName.Password = credentials.Password;
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            return channelFactory;
        }

        public event EventsHandler<bool> ConnectionStateChanged;
    }
}
