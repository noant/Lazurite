﻿using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
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