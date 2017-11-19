using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Net;
using LazuriteMobile.MainDomain;
using Lazurite.MainDomain;

namespace LazuriteMobile.Android.ServiceClient
{
    public class ServiceClientManager: IServiceClientManager
    {
        private static readonly double ConnectionTimeout_Minutes = GlobalSettings.Get(1.0d);

        static ServiceClientManager()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        }

        public IServiceClient Create(ConnectionCredentials credentials)
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

            var endpoint = new EndpointAddress(new Uri(string.Format("https://{0}:{1}/{2}", credentials.Host, credentials.Port, credentials.ServiceName)));

            var client = new ServerClient(binding, endpoint);

            client.ClientCredentials.UserName.UserName = credentials.Login;
            client.ClientCredentials.UserName.Password = credentials.Password;

            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            client.ChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            return new ServiceClientProxy(client);
        }
    }
}