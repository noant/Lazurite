using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.ServiceClient
{
    public struct ConnectionCredentials
    {
        public string Host { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
