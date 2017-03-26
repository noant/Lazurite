using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public class ServerSettings
    {
        public ushort Port { get; set; } = 444;
        public string ServiceName { get; set; } = "PyriteService.svc";
        public string CertificateSubject { get; set; } = "localhost";
        public string SecretKey { get; set; } = "secretKey1234567";
                
        public string GetAddress()
        {
            return string.Format("https://localhost:{0}/{1}", Port, ServiceName);
        }
    }
}
