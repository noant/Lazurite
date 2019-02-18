using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal class ServerClientThreading : IDisposable
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        public ConnectionCredentials Credentials { get; }

        private List<RemoteScenarioInfo> ServerScenariosInfos { get; } = new List<RemoteScenarioInfo>();

        private bool _isListening = false;

        private CancellationTokenSource _timerCancellationToken;

        public ServerClientThreading(ConnectionCredentials credentials)
        {
            Credentials = credentials;
            ServiceClientFactory.Current.ConnectionStateChanged += ClientFactory_ConnectionStateChanged;
        }

        private void ClientFactory_ConnectionStateChanged(object sender, Shared.EventsArgs<bool> args)
        {
            var args2 = ((ServiceClientFactory.ConnectionStateChangedEventArgs)args);
            if (args2.Credentials.Equals(Credentials))
            {
                foreach (var info in ServerScenariosInfos.ToArray())
                {
                    info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, args2.Value));
                }
            }
        }

        public void Append(RemoteScenarioInfo info)
        {
            lock (ServerScenariosInfos)
            {
                if (!ServerScenariosInfos.Any(x => x.Equals(info)))
                {
                    ServerScenariosInfos.Add(info);
                }
            }

            if (!_isListening)
            {
                StartListen();
            }
        }

        public void Remove(RemoteScenarioInfo info)
        {
            lock (ServerScenariosInfos)
            {
                ServerScenariosInfos.Remove(info);
                if (!ServerScenariosInfos.Any())
                {
                    StopListen();
                }
            }
        }

        private void StopListen()
        {
            _timerCancellationToken?.Cancel();
            _isListening = false;
        }

        private void StartListen()
        {
            StopListen();
            _isListening = true;
            _timerCancellationToken = new CancellationTokenSource();
            TaskUtils.StartLongRunning(
                () =>
                {
                    while (!_timerCancellationToken.IsCancellationRequested && ServerScenariosInfos.Any())
                    {
                        for (var i = 0; i < ServerScenariosInfos.Count; i++)
                        {
                            if (i >= ServerScenariosInfos.Count)
                            {
                                i = 0;
                            }

                            if (!ServerScenariosInfos.Any() || _timerCancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            var info = ServerScenariosInfos[i];
                            var error = false;

                            Log.Debug($"Remote scenario refresh iteration begin: [{info.Name}][{info.ScenarioId}]");

                            try
                            {
                                var client = ServiceClientFactory.Current.GetClient(Credentials);
                                var newScenInfo = TaskUtils.Wait(client.GetScenarioInfo(info.ScenarioId));
                                if (!info.Unregistered)
                                {
                                    info.ValueChangedCallback(new RemoteScenarioValueChangedArgs(info, newScenInfo));
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Info($"Ошибка во время соединения с удаленным сценарием. {e.Message}). Сценарий: [{info.Name}]:[{info.ScenarioId}]");
                                error = true;
                                if (!info.Unregistered)
                                {
                                    info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, false));
                                }
                            }

                            Log.Debug($"Remote scenario refresh iteration end: [{info.Name}][{info.ScenarioId}]");

                            if (!ServerScenariosInfos.Any() || _timerCancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            var timeout = error ? CalculateTimeoutOnError() : CalculateTimeout();
                            SystemUtils.Sleep(timeout, _timerCancellationToken.Token);
                        }
                    }
                    StopListen();
                },
                (e) => Log.Error("Ошибка во время прослушивания удаленных сценариев", e));
        }

        private int CalculateTimeout()
        {
            const int minTimeoutMilliseconds = 1000;
            const int maxTimeoutMilliseconds = 5000;
            return minTimeoutMilliseconds + ((maxTimeoutMilliseconds - minTimeoutMilliseconds) / ServerScenariosInfos.Count);
        }

        private int CalculateTimeoutOnError()
        {
            const int minTimeoutMilliseconds = 3000;
            const int maxTimeoutMilliseconds = 10000;
            return minTimeoutMilliseconds + ((maxTimeoutMilliseconds - minTimeoutMilliseconds) / ServerScenariosInfos.Count);
        }

        public void Dispose()
        {
            ServiceClientFactory.Current.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged;
            StopListen();
        }
    }
}