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
        public List<IPermission> Permissions { get; set; } = new List<IPermission>() {
            new DenyForAll()
        };

        public override bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            var result = true;
            foreach (var permission in Permissions)
                result &= permission.IsAvailableForUser(user, source);
            return result;
        }
    }
}
