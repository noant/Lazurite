﻿using Lazurite.MainDomain;
using Lazurite.Security.Permissions;
using System.Collections.Generic;

namespace Lazurite.Security
{
    public class SecuritySettings : SecuritySettingsBase
    {
        public List<IPermission> Permissions { get; set; } = new List<IPermission>() {
            new DenyForNetworkUsage(),
            new DenyForSystemUIUsage()
        };

        public override bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (source == ScenarioStartupSource.System)
                return true;
            var result = true;
            foreach (var permission in Permissions)
                result &= permission.IsAvailableForUser(user, source, action);
            return result;
        }
    }
}
