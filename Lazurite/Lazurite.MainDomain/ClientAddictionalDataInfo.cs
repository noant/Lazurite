using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class ClientAddictionalDataInfo
    {
        public ClientAddictionalDataInfo(UserBase user, string device)
        {
            Device = device;
            CurrentUser = user;
        }

        public string Device { get; private set; }
        public UserBase CurrentUser { get; private set; }
    }
}
