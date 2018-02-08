using Lazurite.MainDomain;
using System;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal class RemoteScenarioInfo
    {
        public RemoteScenarioInfo(
            string name,
            ConnectionCredentials credentials, 
            string scenarioId, 
            Action<RemoteScenarioValueChangedArgs> valueChangedCallback,
            Action<RemoteScenarioAvailabilityChangedArgs> isAvailableChangedCallback)
        {
            Name = name;
            Credentials = credentials;
            ScenarioId = scenarioId;
            ValueChangedCallback = valueChangedCallback ?? throw new ArgumentNullException(nameof(valueChangedCallback));
            IsAvailableChangedCallback = isAvailableChangedCallback ?? throw new ArgumentNullException(nameof(isAvailableChangedCallback));
        }

        public ConnectionCredentials Credentials { get; }
        public string ScenarioId { get; }
        public string Name { get; }
        public Action<RemoteScenarioValueChangedArgs> ValueChangedCallback { get; }
        public Action<RemoteScenarioAvailabilityChangedArgs> IsAvailableChangedCallback { get; }
        public bool Unregistered { get; private set; }

        public void SetUnregistered() => Unregistered = true;
    }
}
