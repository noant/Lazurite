using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    public static partial class RemoteScenarioChangesListener
    {
        private class ServerClientThreading: IDisposable
        {
            private static readonly IClientFactory ClientFactory = Singleton.Resolve<IClientFactory>();

            public ConnectionCredentials Credentials { get; }

            private List<RemoteScenarioInfo> ServerScenariosInfos { get; } = new List<RemoteScenarioInfo>();

            private bool _isListening = false;

            private CancellationTokenSource _timerCancellationToken;

            public ServerClientThreading(ConnectionCredentials credentials)
            {
                Credentials = credentials;
                ClientFactory.ConnectionStateChanged += ClientFactory_ConnectionStateChanged;
            }

            private void ClientFactory_ConnectionStateChanged(object sender, Shared.EventsArgs<bool> args)
            {
                var args2 = ((ConnectionStateChangedEventArgs)args);
                if (args2.Credentials.Equals(Credentials))
                {
                    foreach (var info in ServerScenariosInfos)
                        info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, args2.Value));
                }
            }

            public void Append(RemoteScenarioInfo info)
            {
                if (!ServerScenariosInfos.Any(x => x.Equals(info)))
                    ServerScenariosInfos.Add(info);
                if (!_isListening)
                    StartListen();
            }

            public void Remove(RemoteScenarioInfo info)
            {
                ServerScenariosInfos.Remove(info);
                if (!ServerScenariosInfos.Any())
                    StopListen();
            }

            private void StopListen() {
                _timerCancellationToken?.Cancel();
            }

            private void StartListen() { }

            public void Dispose()
            {
                ClientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged;
                StopListen();
            }
        }
    }
}
