using Pyrite.MainDomain.MessageSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
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
    }
}
