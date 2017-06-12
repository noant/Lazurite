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
        void Initialize(); 

        void StartListenChanges();
        void StopListenChanges();
        void Refresh();
        ClientSettings GetClientSettings();
        void SetClientSettings(ClientSettings settings);

        void ExecuteScenario(string id, string value);

        bool Connected { get; }

        ScenarioInfo[] Scenarios { get; }

        event Action<ScenarioInfo[]> ScenariosChanged;
        event Action NeedRefresh;
        event Action ConnectionLost;
        event Action ConnectionRestored;
        event Action NeedClientSettings;
        event Action LoginOrPasswordInvalid;
        event Action SecretCodeInvalid;
    }
}
