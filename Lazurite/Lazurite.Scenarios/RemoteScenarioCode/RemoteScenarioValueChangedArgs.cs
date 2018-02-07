namespace Lazurite.Scenarios.RemoteScenarioCode
{
    public static partial class RemoteScenarioChangesListener
    {
        public class RemoteScenarioValueChangedArgs
        {
            public RemoteScenarioValueChangedArgs(RemoteScenarioInfo info, string newValue)
            {
                Info = info;
                NewValue = newValue;
            }
            
            public RemoteScenarioInfo Info { get; }
            public string NewValue { get; private set; }
        }
    }
}
