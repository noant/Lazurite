using Lazurite.ActionsDomain.Attributes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для групп...")]
    public class DenyForGroupsPermission : IPermission
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public List<string> GroupsIds { get; set; } = new List<string>();

        public ScenarioAction DenyAction { get; set; } = ScenarioAction.Execute;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return !GroupsIds.Any(x => 
                Repository.Groups.First(g => g.Name.Equals(x))
                .UsersIds.Any(z=>z.Equals(user.Id)));
        }
    }
}
