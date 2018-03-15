using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запрет для любого использования")]
    public class DenyForAll : IPermission
    {
        public ScenarioAction DenyAction { get; set; } = ScenarioAction.ViewValue;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return false;
        }
    }
}
