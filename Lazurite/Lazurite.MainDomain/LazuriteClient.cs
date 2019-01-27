using Lazurite.MainDomain.Statistics;
using SimpleRemoteMethods.ClientSide;
using System.Threading.Tasks;
using System;

namespace Lazurite.MainDomain
{
    public class LazuriteClient
    {
        public Client Client { get; }

        public LazuriteClient(string host, ushort port, bool ssl, string secretKey, string login, string password, TimeSpan timeout = default(TimeSpan))
        {
            Client = new Client(host, port, ssl, secretKey, login, password, timeout);
        }

        public async Task<Boolean> IsScenarioValueChanged(String scenarioId, String lastKnownValue)
        {
            return await Client.CallMethod<Boolean>("IsScenarioValueChanged", scenarioId, lastKnownValue);
        }

        public async Task<ScenarioInfo[]> GetScenariosInfo()
        {
            return await Client.CallMethodArray<ScenarioInfo>("GetScenariosInfo");
        }

        public async Task<ScenarioInfo> GetScenarioInfo(String scenarioId)
        {
            return await Client.CallMethod<ScenarioInfo>("GetScenarioInfo", scenarioId);
        }

        public async Task<String> CalculateScenarioValue(String scenarioId)
        {
            return await Client.CallMethod<String>("CalculateScenarioValue", scenarioId);
        }

        public async Task<String> GetScenarioValue(String scenarioId)
        {
            return await Client.CallMethod<String>("GetScenarioValue", scenarioId);
        }

        public async Task ExecuteScenario(String scenarioId, String value)
        {
            await Client.CallMethod("ExecuteScenario", scenarioId, value);
        }

        public async Task AsyncExecuteScenario(String scenarioId, String value)
        {
            await Client.CallMethod("AsyncExecuteScenario", scenarioId, value);
        }

        public async Task AsyncExecuteScenarioParallel(String scenarioId, String value)
        {
            await Client.CallMethod("AsyncExecuteScenarioParallel", scenarioId, value);
        }

        public async Task<ScenarioInfoLW[]> GetChangedScenarios(DateTime since)
        {
            return await Client.CallMethodArray<ScenarioInfoLW>("GetChangedScenarios", since);
        }

        public async Task SaveVisualSettings(UserVisualSettings visualSettings)
        {
            await Client.CallMethod("SaveVisualSettings", visualSettings);
        }

        public async Task<AddictionalData> SyncAddictionalData(AddictionalData data)
        {
            return await Client.CallMethod<AddictionalData>("SyncAddictionalData", data);
        }

        public async Task<StatisticsScenarioInfo> GetStatisticsInfoForScenario(ScenarioInfo info)
        {
            return await Client.CallMethod<StatisticsScenarioInfo>("GetStatisticsInfoForScenario", info);
        }

        public async Task<StatisticsItem[]> GetStatistics(DateTime since, DateTime to, StatisticsScenarioInfo info)
        {
            return await Client.CallMethodArray<StatisticsItem>("GetStatistics", since, to, info);
        }

        public async Task<ScenarioStatisticsRegistration> GetStatisticsRegistration(String[] scenariosIds)
        {
            return await Client.CallMethod<ScenarioStatisticsRegistration>("GetStatisticsRegistration", scenariosIds);
        }
    }
}
