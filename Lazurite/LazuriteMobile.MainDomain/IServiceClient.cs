using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazuriteMobile.MainDomain
{
    public interface IServiceClient
    {
        IAsyncResult BeginIsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue, AsyncCallback callback, object asyncState);

        bool EndIsScenarioValueChanged(IAsyncResult result);
        
        IAsyncResult BeginGetScenariosInfo(AsyncCallback callback, object asyncState);

        EncryptedList<ScenarioInfo> EndGetScenariosInfo(IAsyncResult result);

        IAsyncResult BeginGetScenarioInfo(Encrypted<string> scenarioId, AsyncCallback callback, object asyncState);

        Encrypted<Lazurite.MainDomain.ScenarioInfo> EndGetScenarioInfo(IAsyncResult result);

        IAsyncResult BeginCalculateScenarioValue(Encrypted<string> scenarioId, AsyncCallback callback, object asyncState);

        Encrypted<string> EndCalculateScenarioValue(System.IAsyncResult result);
        
        IAsyncResult BeginGetScenarioValue(Lazurite.MainDomain.MessageSecurity.Encrypted<string> scenarioId, System.AsyncCallback callback, object asyncState);

        Encrypted<string> EndGetScenarioValue(System.IAsyncResult result);
                
        IAsyncResult BeginExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value, System.AsyncCallback callback, object asyncState);

        void EndExecuteScenario(IAsyncResult result);
        
        IAsyncResult BeginAsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value, System.AsyncCallback callback, object asyncState);

        void EndAsyncExecuteScenario(IAsyncResult result);
        
        IAsyncResult BeginAsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value, System.AsyncCallback callback, object asyncState);

        void EndAsyncExecuteScenarioParallel(IAsyncResult result);
                
        IAsyncResult BeginGetChangedScenarios(DateTime since, AsyncCallback callback, object asyncState);

        EncryptedList<ScenarioInfoLW> EndGetChangedScenarios(System.IAsyncResult result);
                
        IAsyncResult BeginSaveVisualSettings(Encrypted<UserVisualSettings> visualSettings, System.AsyncCallback callback, object asyncState);

        void EndSaveVisualSettings(System.IAsyncResult result);

        void Close();
    }
}
