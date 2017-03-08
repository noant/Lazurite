using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Security.Permissions
{
    public interface IPermission
    {
        bool IsAvailableForUser(UserBase user, ScenarioStartupSource source);
    }
}
