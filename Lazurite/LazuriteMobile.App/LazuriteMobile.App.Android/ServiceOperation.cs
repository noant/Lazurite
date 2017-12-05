namespace LazuriteMobile.App.Droid
{
    public enum ServiceOperation
    {
        GetClientSettings = 1,
        SetClientSettings = 2,
        ExecuteScenario = 4,
        GetIsConnected = 8,
        ScenariosChanged = 16,
        NeedRefresh = 32,
        ConnectionLost = 64,
        ConnectionRestored = 128,
        NeedClientSettings = 256,
        CredentialsInvalid = 512,
        SecretCodeInvalid = 1024,
        CredentialsLoaded = 2048,
        GetScenarios = 4096,
        Initialize = 8192,
        ConnectionError = 16384,
        ReConnect = 32768
    }
}