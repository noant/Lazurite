using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.Android.ServiceClient
{
    public class ServiceClientProxy : IServiceClient
    {
        private ServerClient _client;
        private LazuriteMobile.Android.ServiceClient.IServer _channel; 
        public ServiceClientProxy(ServerClient baseClient)
        {
            _client = baseClient;
            _channel = (LazuriteMobile.Android.ServiceClient.IServer)_client.InnerChannel;
        }

        public IAsyncResult BeginAsyncExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginAsyncExecuteScenario(scenarioId, value, callback, asyncState);
        }

        public IAsyncResult BeginAsyncExecuteScenarioParallel(Encrypted<string> scenarioId, Encrypted<string> value, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginAsyncExecuteScenarioParallel(scenarioId, value, callback, asyncState);
        }

        public IAsyncResult BeginCalculateScenarioValue(Encrypted<string> scenarioId, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginCalculateScenarioValue(scenarioId, callback, asyncState);
        }

        public IAsyncResult BeginExecuteScenario(Encrypted<string> scenarioId, Encrypted<string> value, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginExecuteScenario(scenarioId, value, callback, asyncState);
        }

        public IAsyncResult BeginGetChangedScenarios(DateTime since, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginGetChangedScenarios(since, callback, asyncState);
        }

        public IAsyncResult BeginGetScenarioInfo(Encrypted<string> scenarioId, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginGetScenarioInfo(scenarioId, callback, asyncState);
        }

        public IAsyncResult BeginGetScenariosInfo(AsyncCallback callback, object asyncState)
        {
            return _channel.BeginGetScenariosInfo(callback, asyncState);
        }

        public IAsyncResult BeginGetScenarioValue(Encrypted<string> scenarioId, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginGetScenarioValue(scenarioId, callback, asyncState);
        }

        public IAsyncResult BeginIsScenarioValueChanged(Encrypted<string> scenarioId, Encrypted<string> lastKnownValue, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginIsScenarioValueChanged(scenarioId, lastKnownValue, callback, asyncState);
        }

        public IAsyncResult BeginSaveVisualSettings(Encrypted<UserVisualSettings> visualSettings, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginSaveVisualSettings(visualSettings, callback, asyncState);
        }

        public void EndAsyncExecuteScenario(IAsyncResult result)
        {
            _channel.EndAsyncExecuteScenario(result);
        }

        public void EndAsyncExecuteScenarioParallel(IAsyncResult result)
        {
            _channel.EndAsyncExecuteScenarioParallel(result);
        }

        public Encrypted<string> EndCalculateScenarioValue(IAsyncResult result)
        {
            return _channel.EndCalculateScenarioValue(result);
        }

        public void EndExecuteScenario(IAsyncResult result)
        {
            _channel.EndExecuteScenario(result);
        }

        public EncryptedList<ScenarioInfoLW> EndGetChangedScenarios(IAsyncResult result)
        {
            return _channel.EndGetChangedScenarios(result);
        }

        public Encrypted<ScenarioInfo> EndGetScenarioInfo(IAsyncResult result)
        {
            return _channel.EndGetScenarioInfo(result);
        }

        public EncryptedList<ScenarioInfo> EndGetScenariosInfo(IAsyncResult result)
        {
            return _channel.EndGetScenariosInfo(result);
        }

        public Encrypted<string> EndGetScenarioValue(IAsyncResult result)
        {
            return _channel.EndGetScenarioValue(result);
        }

        public bool EndIsScenarioValueChanged(IAsyncResult result)
        {
            return _channel.EndIsScenarioValueChanged(result);
        }

        public void EndSaveVisualSettings(IAsyncResult result)
        {
            _channel.EndIsScenarioValueChanged(result);
        }

        public IAsyncResult BeginSyncAddictionalData(Encrypted<AddictionalData> data, AsyncCallback callback, object asyncState)
        {
            return _channel.BeginSyncAddictionalData(data, callback, asyncState);
        }

        public Encrypted<AddictionalData> EndSyncAddictionalData(IAsyncResult result)
        {
            return _channel.EndSyncAddictionalData(result);
        }

        public void Close()
        {
            try
            {
                if (!IsClosedOrFaulted)
                    _client.Close();
            }
            catch
            {
                //do nothing
            }
        }

        public bool IsClosedOrFaulted
        {
            get
            {
                return _client.State == System.ServiceModel.CommunicationState.Closed ||
                    _client.State == System.ServiceModel.CommunicationState.Closing ||
                    _client.State == System.ServiceModel.CommunicationState.Faulted;
            }
        }
    }
}