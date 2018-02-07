using Lazurite.MainDomain;
using System;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    public static partial class RemoteScenarioChangesListener
    {
        public class RemoteScenarioInfo
        {
            public RemoteScenarioInfo(ConnectionCredentials credentials, 
                string scenarioId, 
                Action<RemoteScenarioValueChangedArgs> valueChangedCallback,
                Action<RemoteScenarioAvailabilityChangedArgs> isAvailableChangedCallback)
            {
                Credentials = credentials;
                ScenarioId = scenarioId;
                ValueChangedCallback = valueChangedCallback;
                IsAvailableChangedCallback = isAvailableChangedCallback;
            }

            public ConnectionCredentials Credentials { get; }
            public string ScenarioId { get; }
            public Action<RemoteScenarioValueChangedArgs> ValueChangedCallback { get; }
            public Action<RemoteScenarioAvailabilityChangedArgs> IsAvailableChangedCallback { get; }
        }
    }
}
