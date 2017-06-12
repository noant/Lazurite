using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public class ClientSettings
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ServerSecretCode { get; set; }
        public string ServerHost { get; set; }
        public string ServerService { get; set; }
        public ushort ServerPort { get; set; }
    }
}
