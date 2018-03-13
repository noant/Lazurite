namespace Lazurite.MainDomain
{
    public abstract class SecuritySettingsBase
    {
        public abstract bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action);
    }
}
