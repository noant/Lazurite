using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public interface IServiceClientManager
    {
        IServiceClient Create(string host, ushort port, string serviceName, string secretKey, string userLogin, string password);
    }
}
