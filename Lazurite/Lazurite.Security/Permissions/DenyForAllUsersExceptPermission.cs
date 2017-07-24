using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех пользователей, кроме...")]
    public class DenyForAllUsersExceptPermission : IPermission
    {
        public List<UserBase> Users { get; set; } = new List<UserBase>();
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return Users.Any(x => x.Id.Equals(user.Id));
        }
    }
}
