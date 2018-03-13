using Lazurite.ActionsDomain.Attributes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех групп, кроме...")]
    public class DenyForAllGroupsExceptPermission : IPermission
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public List<string> GroupsIds { get; set; } = new List<string>();

        public ScenarioAction DenyAction { get; set; } = ScenarioAction.Execute;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return user is SystemUser || GroupsIds.Any(x => 
                Repository.Groups.First(z=>z.Name.Equals(x))
                .UsersIds.Any(z => z.Equals(user.Id)));
        }
    }
}
