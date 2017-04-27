using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class UserGroupBase
    {
        public string Name { get; set; }

        public List<UserBase> Users { get; set; }
    }
}
