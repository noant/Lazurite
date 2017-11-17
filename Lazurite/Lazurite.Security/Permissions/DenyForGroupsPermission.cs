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
    [HumanFriendlyName("Запретить для групп...")]
    public class DenyForGroupsPermission : IPermission
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public List<string> GroupsIds { get; set; } = new List<string>();

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return !GroupsIds.Any(x => 
                Repository.Groups.First(g => g.Name.Equals(x))
                .UsersIds.Any(z=>z.Equals(user.Id)));
        }
    }
}
