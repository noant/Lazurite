using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public interface IServiceClientManager
    {
        IServiceClient Create(ConnectionCredentials credentials);
    }
}
