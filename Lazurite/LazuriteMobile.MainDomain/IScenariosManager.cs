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
        void Initialize(Action<bool> callback); 
        
        void GetClientSettings(Action<ClientSettings> callback);
        void SetClientSettings(ClientSettings settings);
        void ExecuteScenario(ExecuteScenarioArgs args);
        void IsConnected(Action<bool> callback);
        void GetScenarios(Action<ScenarioInfo[]> callback);
        void Close();

        event Action<ScenarioInfo[]> ScenariosChanged;
        event Action NeedRefresh;
        event Action ConnectionLost;
        event Action ConnectionRestored;
        event Action NeedClientSettings;
        event Action LoginOrPasswordInvalid;
        event Action SecretCodeInvalid;
        event Action CredentialsLoaded;
    }

    public class ExecuteScenarioArgs
    {
        public ExecuteScenarioArgs() { }

        public ExecuteScenarioArgs(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public string Id { get; set; }
        public string Value { get; set; }
    }
}
