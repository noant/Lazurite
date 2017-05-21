using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public interface IScenariosManager
    {
        void Initialize(string host, ushort port, string serviceName, string login, string password, string secretKey); 

        void StartListenChanges();
        void StopListenChanges();
        void Refresh();

        void ExecuteScenario(string id, string value);

        bool Connected { get; }

        ScenarioInfo[] Scenarios { get; }

        event Action<ScenarioInfo[]> ScenariosChanged;       
        event Action<ScenarioInfo[]> NewScenarios;
        event Action ConnectionLost;
        event Action ConnectionRestored;
    }
}
