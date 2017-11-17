using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для пользователей...")]
    public class DenyForUsersPermission : IPermission
    {
        public List<string> UsersIds { get; set; } = new List<string>();
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return !UsersIds.Any(x => x.Equals(user.Id));
        }
    }
}