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
        public ushort Port { get; set; }
        public bool UseCustomCertificate { get; set; }
    }
}
