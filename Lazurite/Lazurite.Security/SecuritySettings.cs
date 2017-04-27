using Lazurite.MainDomain;
using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Security
{
    public class SecuritySettings : SecuritySettingsBase
    {
        public List<IPermission> Permissions = new List<IPermission>();
        public override bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            var result = true;
            foreach (var permission in Permissions)
                result &= permission.IsAvailableForUser(user, source);
            return result;
        }
    }
}
