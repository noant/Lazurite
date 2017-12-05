using Lazurite.MainDomain;

namespace Lazurite.CoreActions.CoreActions
{
    public interface ICoreAction
    {
        string TargetScenarioId { get; }
        ScenarioBase GetTargetScenario();
        void SetTargetScenario(ScenarioBase scenario);
    }
}