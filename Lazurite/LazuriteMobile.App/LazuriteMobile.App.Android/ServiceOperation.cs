namespace LazuriteMobile.App.Droid
{
    public enum ServiceOperation : byte
    {
        GetClientSettings = 1,
        SetClientSettings = 2,
        ExecuteScenario = 3,
        GetIsConnected = 4,
        ScenariosChanged = 5,
        NeedRefresh = 6,
        ConnectionLost = 7,
        ConnectionRestored = 8,
        NeedClientSettings = 9,
        CredentialsInvalid = 10,
        SecretCodeInvalid = 11,
        CredentialsLoaded = 12,
        GetScenarios = 13,
        Initialize = 14,
        ConnectionError = 15,
        ReConnect = 16,
        RefreshIteration = 17,
        GetNotifications = 18,
        ScreenOnActions = 19,
        BruteforceSuspition = 20,
        GetListenerSettings = 21,
        SetListenerSettings = 22,
        GetGeolocationListenerSettings = 23,
        SetGeolocationListenerSettings = 24,
        GetGeolocationAccuracy = 25,
        SetGeolocationAccuracy = 26,
        Close = 27,
    }
}