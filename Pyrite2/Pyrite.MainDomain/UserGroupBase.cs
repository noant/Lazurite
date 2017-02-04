using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public class UserGroupBase
    {
        public string Name { get; set; }

        public UserBase[] Users { get; set; }
    }
}
