using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить использовать из другого сценария")]
    public class DenyForScenarioUsePermission : IPermission
    {
        public ScenarioAction DenyAction { get; set; } = ScenarioAction.Execute;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return source != ScenarioStartupSource.OtherScenario;
        }
    }
}
