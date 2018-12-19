using Lazurite.MainDomain;
using System;

namespace LazuriteMobile.MainDomain
{
    public interface IScenariosManager
    {
        void Initialize(Action<bool> callback);

        void ReConnect();
        void GetClientSettings(Action<ConnectionCredentials> callback);
        void SetClientSettings(ConnectionCredentials settings);
        void ExecuteScenario(ExecuteScenarioArgs args);
        void IsConnected(Action<ManagerConnectionState> callback);
        void GetScenarios(Action<ScenarioInfo[]> callback);
        void GetNotifications(Action<LazuriteNotification[]> notifications);
        void Close();
        void RefreshIteration();
        void ScreenOnActions();

        event Action<ScenarioInfo[]> ScenariosChanged;
        event Action NeedRefresh;
        event Action ConnectionLost;
        event Action ConnectionRestored;
        event Action NeedClientSettings;
        event Action LoginOrPasswordInvalid;
        event Action AccessLocked;
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

    public enum ManagerConnectionState
    {
        Connected,
        Disconnected,
        Connecting
    }
}
