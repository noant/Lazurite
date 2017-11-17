using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;
using Lazurite.IOC;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех групп, кроме...")]
    public class DenyForAllGroupsExceptPermission : IPermission
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public List<string> GroupsIds { get; set; } = new List<string>();
        
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return user is SystemUser || GroupsIds.Any(x => 
                Repository.Groups.First(z=>z.Name.Equals(x))
                .UsersIds.Any(z => z.Equals(user.Id)));
        }
    }
}
