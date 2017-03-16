using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public class ServerSettings
    {
        public string CertificateIssuer { get; set; }
        public ushort Port { get; set; } = 443;
        public string ServiceName { get; set; } = "PyriteService";
        
        public CertificateLoadMode CertificateLoadMode { get; set; }

        public string CertificatePath { get; set; }
        public string CertificateSubjectName { get; set; }
        public string CertificatePassword { get; internal set; }

        public string GetAddress()
        {
            return string.Format("https://localhost:{0}/{1}", Port, ServiceName);
        }
    }
}
