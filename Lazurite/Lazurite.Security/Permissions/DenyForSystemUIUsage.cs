using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для меню быстрого запуска")]
    public class DenyForSystemUIUsage : IPermission
    {
        public ScenarioAction DenyAction { get; set; } = ScenarioAction.ViewValue;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return source != ScenarioStartupSource.SystemUI;
        }
    }
}
