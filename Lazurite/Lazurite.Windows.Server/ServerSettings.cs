using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Server
{
    public class ServerSettings
    {
        public ushort Port { get; set; } = 8080;
        public string ServiceName { get; set; } = "Lazurite";
        public string CertificateHash { get; set; }
        public string SecretKey { get; set; } = "0123456789123456";
                
        public string GetAddress()
        {
            return string.Format("https://localhost:{0}/{1}", Port, ServiceName);
        }
    }
}