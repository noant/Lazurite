using Lazurite.MainDomain;
using System;

namespace LazuriteMobile.MainDomain
{
    public interface IClientManager
    {
        void Initialize(Action<bool> callback);

        void ReConnect();

        void ReInitialize();

        void GetListenerSettings(Action<ListenerSettings> callback);

        void SetListenerSettings(ListenerSettings settings);

        void GetGeolocationListenerSettings(Action<GeolocationListenerSettings> callback);

        void SetGeolocationListenerSettings(GeolocationListenerSettings settings);

        void GetGeolocationAccuracy(Action<int> callback);

        void SetGeolocationAccuracy(int accuracyMeters);

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

        event Action BruteforceSuspition;

        event Action SecretCodeInvalid;

        event Action ConnectionError;

        event Action CredentialsLoaded;
    }

    public class ExecuteScenarioArgs
    {
        public ExecuteScenarioArgs()
        {
            // Do nothing
        }

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