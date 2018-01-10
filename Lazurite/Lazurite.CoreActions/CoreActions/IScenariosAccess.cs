using Lazurite.MainDomain;

namespace Lazurite.CoreActions.CoreActions
{
    public interface IScenariosAccess
    {
        string TargetScenarioId { get; }
        ScenarioBase GetTargetScenario();
        void SetTargetScenario(ScenarioBase scenario);
    }
}