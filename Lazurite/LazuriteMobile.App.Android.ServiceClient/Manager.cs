using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;
using System.Net;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace LazuriteMobile.App.Android.ServiceClient
{
    public class Manager : IClientManager
    {
        private static readonly double ConnectionTimeout_Minutes = 2.5;
        
        static Manager()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        }

        private ChannelFactory<IServer> _channelFactory;

        public void CreateConnection(ConnectionCredentials credentials)
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

            _channelFactory = new ChannelFactory<IServer>(binding, endpoint);

            _channelFactory.Credentials.UserName.UserName = credentials.Login;
            _channelFactory.Credentials.UserName.Password = credentials.Password;
            _channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
        }

        public bool IsClosed()
        {
            if (_channelFactory == null)
                return false;
            return _channelFactory.State == CommunicationState.Faulted ||
                _channelFactory.State == CommunicationState.Closed ||
                _channelFactory.State == CommunicationState.Closing;
        }

        public void Close()
        {
            _channelFactory?.Close();
        }

        public void Close(IServer client) => ((ICommunicationObject)client).Close();

        public void Abort(IServer client) => ((ICommunicationObject)client).Abort();
        
        public IServer GetActualInstance() => _channelFactory.CreateChannel();
    }
}
