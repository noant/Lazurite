using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Security
{
    public class SystemUser: User
    {
        public SystemUser()
        {
            Id = "0";
            Name = "Системный пользователь";
        }
    }
}
