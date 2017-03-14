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
        bool IsScenarioValueChanged(string scenarioId, string lastKnownValue);
        [OperationContract]
        ScenarioInfo[] GetScenariosInfo();
        [OperationContract]
        ScenarioInfo GetScenarioInfo(string scenarioId);
        [OperationContract]
        string CalculateScenarioValue(string scenarioId);
        [OperationContract]
        string GetScenarioValue(string scenarioId);
        [OperationContract]
        void ExecuteScenario(string scenarioId, string value);
        [OperationContract]
        void AsyncExecuteScenario(string scenarioId, string value);
        [OperationContract]
        void AsyncExecuteScenarioParallel(string scenarioId, string value);
        [OperationContract]
        ScenarioInfoLW[] GetChangedScenarios(DateTime since);
    }
}
