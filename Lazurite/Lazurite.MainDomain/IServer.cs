using Lazurite.MainDomain.Statistics;
using SimpleRemoteMethods.Bases;
using System;

namespace Lazurite.MainDomain
{
    public interface IServer
    {
        [Remote]
        bool IsScenarioValueChanged(string scenarioId, string lastKnownValue);
        [Remote]
        ScenarioInfo[] GetScenariosInfo();
        [Remote]
        ScenarioInfo GetScenarioInfo(string scenarioId);
        [Remote]
        string CalculateScenarioValue(string scenarioId);
        [Remote]
        string GetScenarioValue(string scenarioId);
        [Remote]
        void ExecuteScenario(string scenarioId, string value);
        [Remote]
        void AsyncExecuteScenario(string scenarioId, string value);
        [Remote]
        void AsyncExecuteScenarioParallel(string scenarioId, string value);
        [Remote]
        ScenarioInfoLW[] GetChangedScenarios(DateTime since);
        [Remote]
        void SaveVisualSettings(UserVisualSettings visualSettings);
        [Remote]
        AddictionalData SyncAddictionalData(AddictionalData data);
        [Remote]
        StatisticsScenarioInfo GetStatisticsInfoForScenario(ScenarioInfo info);
        [Remote]
        StatisticsItem[] GetStatistics(DateTime since, DateTime to, StatisticsScenarioInfo info);
        [Remote]
        ScenarioStatisticsRegistration GetStatisticsRegistration(string[] scenariosIds);
    }
}
