﻿using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    public interface IPermission
    {
        bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action);
        ScenarioAction DenyAction { get; set; }
    }
}
