using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.ServiceClient
{
    public struct ConnectionCredentials
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string ServiceName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SecretKey { get; set; }
        
        public string GetAddress()
        {
            return string.Format("https://{0}:{1}/{2}", Host, Port, ServiceName);
        }
    }
}
