namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal class RemoteScenarioAvailabilityChangedArgs
    {
        public RemoteScenarioAvailabilityChangedArgs(RemoteScenarioInfo info, bool isAvailable)
        {
            Info = info;
            IsAvailable = isAvailable;
        }

        public RemoteScenarioInfo Info { get; }
        public bool IsAvailable { get; private set; }
    }
}
