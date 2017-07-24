using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех групп, кроме...")]
    public class DenyForAllGroupsExceptPermission : IPermission
    {
        public List<UserGroupBase> Groups { get; set; } = new List<UserGroupBase>();
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return Groups.Any(x => x.Users.Any(z => z.Id.Equals(user.Id)));
        }
    }
}
