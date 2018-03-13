using Lazurite.MainDomain;
using Lazurite.Security.Permissions;
using System.Collections.Generic;

namespace Lazurite.Security
{
    public class SecuritySettings : SecuritySettingsBase
    {
        public List<IPermission> Permissions { get; set; } = new List<IPermission>() {
            new DenyForAll()
        };

        public override bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            var result = true;
            foreach (var permission in Permissions)
                result &= permission.IsAvailableForUser(user, source, action);
            return result;
        }
    }
}
