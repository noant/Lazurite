using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.MainDomain;

namespace Pyrite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех групп и пользователей")]
    public class DenyForAll : IPermission
    {
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return false;
        }
    }
}
