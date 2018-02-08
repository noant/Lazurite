using Lazurite.MainDomain;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal class RemoteScenarioValueChangedArgs
    {
        public RemoteScenarioValueChangedArgs(RemoteScenarioInfo info, ScenarioInfo scenInfo)
        {
            Info = info;
            ScenarioInfo = scenInfo;
        }
            
        public RemoteScenarioInfo Info { get; }
        public ScenarioInfo ScenarioInfo { get; private set; }
    }
}
