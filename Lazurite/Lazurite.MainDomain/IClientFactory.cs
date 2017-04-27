using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public interface IClientFactory
    {
        IServer GetServer(string host, ushort port, string serviceName, string secretKey, string userLogin, string password);
    }
}
