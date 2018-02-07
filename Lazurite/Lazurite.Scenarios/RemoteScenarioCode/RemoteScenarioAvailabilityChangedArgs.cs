namespace Lazurite.Scenarios.RemoteScenarioCode
{
    public static partial class RemoteScenarioChangesListener
    {
        public class RemoteScenarioAvailabilityChangedArgs
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
}
