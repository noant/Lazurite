using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class UserGroupBase
    {
        private static readonly UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public string Name { get; set; }

        public List<string> UsersIds { get; set; } = new List<string>();

        public UserBase[] GetUsers()
        {
            return Repository.Users.Where(x => UsersIds.Contains(x.Id)).ToArray();
        }
    }
}
