using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
        GetScenarios = 4096
    }
}