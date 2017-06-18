using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.MainDomain;
using Lazurite.ActionsDomain.Attributes;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для удаленного запуска")]
    public class DenyForPublicUsagePermission : IPermission
    {
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return source != ScenarioStartupSource.PublicUsage;
        }
    }
}
