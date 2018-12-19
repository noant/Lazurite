using Lazurite.MainDomain.MessageSecurity;
using Lazurite.MainDomain.Statistics;
using Lazurite.Shared;
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
        EncryptedList<ScenarioInfoLW> GetChangedScenarios(SafeDateTime since);
        [OperationContract]
        void SaveVisualSettings(Encrypted<UserVisualSettings> visualSettings);
        [OperationContract]
        Encrypted<AddictionalData> SyncAddictionalData(Encrypted<AddictionalData> data);
        [OperationContract]
        Encrypted<StatisticsScenarioInfo> GetStatisticsInfoForScenario(Encrypted<ScenarioInfo> info);
        [OperationContract]
        EncryptedList<StatisticsItem> GetStatistics(SafeDateTime since, SafeDateTime to, Encrypted<StatisticsScenarioInfo> info);
        [OperationContract]
        Encrypted<ScenarioStatisticsRegistration> GetStatisticsRegistration(EncryptedList<string> scenariosIds);
    }
}
