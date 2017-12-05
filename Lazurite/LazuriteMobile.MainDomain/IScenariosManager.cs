using Lazurite.MainDomain;
using System;

namespace LazuriteMobile.MainDomain
{
    public interface IScenariosManager
    {
        void Initialize(Action<bool> callback); 
        
        void GetClientSettings(Action<ConnectionCredentials> callback);
        void SetClientSettings(ConnectionCredentials settings);
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
        event Action ConnectionError;
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
