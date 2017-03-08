using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.MainDomain;

namespace Pyrite.Security.Permissions
{
    [HumanFriendlyName("Запретить для групп")]
    public class DenyForGroupsPermission : IPermission
    {
        public List<UserGroupBase> Groups { get; set; }
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return !Groups.Any(x => x.Users.Any(z=>z.Id.Equals(user.Id)));
        }
    }
}
