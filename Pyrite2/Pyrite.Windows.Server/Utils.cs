using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public static class Utils
    {
        public static void NetshAddSslCert(ServerSettings settings)
        {
            var store = new X509Store("My", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store
                .Certificates
                .Cast<X509Certificate2>()
                .FirstOrDefault(x => x.Subject.Equals("CN=" + settings.CertificateSubject));

            if (cert == null)
                throw new Exception(string.Format("Cannot found certificate [{0}]", settings.CertificateSubject));

            var certhash = cert.GetCertHashString();
            var appid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;
            var commandString = string.Format(" http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={{{2}}}", settings.Port, certhash, appid);
            
            var output = 
                Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), commandString);

            var outputLower = output.ToLower();
            if (!outputLower.Contains("183") && (outputLower.Contains("error") || outputLower.Contains("ошибка") || outputLower.Contains("сбой") || outputLower.Contains("неверно")))
                throw new Exception(output);
        }

        public static void NetshDeleteSslCert(ushort port)
        {
            var command = " http delete sslcert ipport=0.0.0.0:" + port;
            var result = Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), command);
        }

        public static void NetshAddUrlacl(ServerSettings settings)
        {
            var commandString = string.Format(@" http add urlacl url={0} user={1}\{2}", settings.GetAddress(), Environment.UserDomainName, Environment.UserName);

            var output = Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), commandString);
            
            var outputLower = output.ToLower();
            if (!outputLower.Contains("183") && (outputLower.Contains("error") || outputLower.Contains("ошибка") || outputLower.Contains("сбой") || outputLower.Contains("неверно")))
                throw new Exception(output);
        }

        public static void NetshDeleteUrlacl(ServerSettings settings)
        {
            var commandString = string.Format(@" http delete urlacl url={0}", settings.GetAddress().Replace("https://", "http://"));
            var output = Windows.Utils.Utils.ExecuteProcess(Path.Combine(Environment.SystemDirectory, "netsh.exe"), commandString);
        }
    }
}
