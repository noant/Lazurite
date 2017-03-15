﻿using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.MainDomain;

namespace Pyrite.Security.Permissions
{
    [HumanFriendlyName("Запретить для все пользователей кроме")]
    public class DenyForAllUsersExceptPermission : IPermission
    {
        public List<UserBase> Users { get; set; }
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return Users.Any(x => x.Id.Equals(user.Id));
        }
    }
}