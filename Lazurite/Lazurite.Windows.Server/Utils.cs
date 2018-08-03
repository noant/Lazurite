using SslCertBinding.Net;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Lazurite.Windows.Server
{
    public static class Utils
    {
        public static void NetshAddSslCert(string certificateHash, ushort port)
        {
            NetshDeleteSslCert(port);
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store
                .Certificates
                .Cast<X509Certificate2>()
                .FirstOrDefault(x => x.GetCertHashString().Equals(certificateHash));

            if (cert == null)
                throw new Exception(string.Format("Cannot found certificate [{0}]", certificateHash));
            
            var appid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;

            var certificateBindingConfiguration = new CertificateBindingConfiguration();

            certificateBindingConfiguration.Bind(
                new CertificateBinding(
                    certificateHash,
                    StoreName.My,
                    new IPEndPoint(new IPAddress(new byte[] { 0, 0, 0, 0 }), port),
                    Guid.Parse(appid))
            );
        }

        public static CertificateInfo[] GetInstalledCertificates()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            return store
                .Certificates
                .Cast<X509Certificate2>().Select(x => new CertificateInfo() {
                    Hash = x.GetCertHashString(),
                    Description = x.IssuerName.Name + " / " + x.SubjectName.Name
                }).ToArray();
        }

        public static void NetshDeleteSslCert(ushort port)
        {
            var command = " http delete sslcert ipport=0.0.0.0:" + port;
            var result = Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), command);
        }

        public static void NetshAddUrlacl(string address)
        {
            NetshDeleteUrlacl(address);
            var commandString = string.Format(@" http add urlacl url={0} user={1}\{2}", address, Environment.UserDomainName, Environment.UserName);

            var output = Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), commandString);
            
            var outputLower = output.ToLower();
            if (!outputLower.Contains("183") && (outputLower.Contains("error") || outputLower.Contains("ошибка") || outputLower.Contains("сбой") || outputLower.Contains("неверно")))
                throw new Exception(output);
        }

        public static void NetshDeleteUrlacl(string address)
        {
            var commandString = string.Format(@" http delete urlacl url={0}", address.Replace("https://", "http://"));
            Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), commandString);
        }

        public static string AddCertificate(string filename, string password)
        {
            var certificate = new X509Certificate2(filename, password);
            var name = certificate.Subject.Replace("CN=", "");
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);

            var cert = new X509Certificate2(certificate);
            store.Add(cert);
            store.Close();
            return cert.GetCertHashString();
        }

        public static void NetshAllowPort(ushort port)
        {
            var ruleName = "Lazurite" + Windows.Utils.Utils.GetCurrentLazuriteUniqueHash();
            var commandRemove = string.Format(" advfirewall firewall delete rule name = \"{0}\"", ruleName);
            var commandAdd = string.Format(" firewall add portopening TCP {0} {1} enable ALL", port, ruleName);
            var netshpath = Path.Combine(Environment.SystemDirectory, "netsh.exe");
            Windows.Utils.Utils.ExecuteProcess(netshpath, commandRemove);
            Windows.Utils.Utils.ExecuteProcess(netshpath, commandAdd);
        }

        public class CertificateInfo
        {
            public string Description { get; set; }
            public string Hash { get; set; }
        }
    }
}
