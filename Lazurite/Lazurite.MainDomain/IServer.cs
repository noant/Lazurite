using Lazurite.MainDomain.MessageSecurity;
using Lazurite.MainDomain.Statistics;
using System;
using System.ServiceModel;

namespace Lazurite.MainDomain
{
    [ServiceContract]
    public interface IServer
    {
        [OperationContract]
        bool IsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue);
        [OperationContract]
        EncryptedList<ScenarioInfo> GetScenariosInfo();
        [OperationContract]
        Encrypted<ScenarioInfo> GetScenarioInfo(Encrypted<string> scenarioId);
        [OperationContract]
        Encrypted<string> CalculateScenarioValue(Encrypted<string> scenarioId);
        [OperationContract]
        Encrypted<string> GetScenarioValue(Encrypted<string> scenarioId);
        [OperationContract]
        void ExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value);
        [OperationContract]
        void AsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value);
        [OperationContract]
        void AsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value);
        [OperationContract]
        EncryptedList<ScenarioInfoLW> GetChangedScenarios(DateTime since);
        [OperationContract]
        void SaveVisualSettings(Encrypted<UserVisualSettings> visualSettings);
        [OperationContract]
        Encrypted<AddictionalData> SyncAddictionalData(Encrypted<AddictionalData> data);
        [OperationContract]
        Encrypted<StatisticsScenarioInfo> GetStatisticsInfoForScenario(Encrypted<ScenarioInfo> info);
        [OperationContract]
        EncryptedList<StatisticsItem> GetStatistics(DateTime since, DateTime to, Encrypted<StatisticsScenarioInfo> info);
        [OperationContract]
        bool IsStatisticsRegistered(Encrypted<StatisticsScenarioInfo> info);
    }
}
