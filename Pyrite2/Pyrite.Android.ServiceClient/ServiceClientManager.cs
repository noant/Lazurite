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
using Pyrite.MainDomain;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Net;

namespace Pyrite.Android.ServiceClient
{
    public static class ServiceClientManager
    {
        static ServiceClientManager()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        }

        public static PyriteServiceClient Create(string host, ushort port, string serviceName, string userLogin, string password)
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
            var endpoint = new EndpointAddress(new Uri(string.Format("https://{0}:{1}/{2}", host, port, serviceName)));

            var client = new PyriteServiceClient(binding, endpoint);
            
            client.ClientCredentials.UserName.UserName = userLogin;
            client.ClientCredentials.UserName.Password = password;

            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            client.ChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            return client;
        }
    }
}