using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех групп и пользователей")]
    public class DenyForAll : IPermission
    {
        public ScenarioAction DenyAction { get; set; } = ScenarioAction.Execute;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return false;
        }
    }
}
