using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Server
{
    public class ServerSettings
    {
        public ushort Port { get; set; }
        public string ServiceName { get; set; }
        public string CertificateSubject { get; set; }
        public string SecretKey { get; set; }
                
        public string GetAddress()
        {
            return string.Format("https://localhost:{0}/{1}", Port, ServiceName);
        }
    }
}
