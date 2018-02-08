using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.MessageSecurity;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal class ServerClientThreading: IDisposable
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
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
                foreach (var info in ServerScenariosInfos.ToArray())
                    info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, args2.Value));
            }
        }

        public void Append(RemoteScenarioInfo info)
        {
            lock (ServerScenariosInfos)
                if (!ServerScenariosInfos.Any(x => x.Equals(info)))
                    ServerScenariosInfos.Add(info);
            if (!_isListening)
                StartListen();
        }

        public void Remove(RemoteScenarioInfo info)
        {
            lock (ServerScenariosInfos)
            {
                ServerScenariosInfos.Remove(info);
                if (!ServerScenariosInfos.Any())
                    StopListen();
            }
        }

        private void StopListen()
        {
            _timerCancellationToken?.Cancel();
            _isListening = false;
        }

        private void StartListen() {
            StopListen();
            _isListening = true;
            _timerCancellationToken = new CancellationTokenSource();
            TaskUtils.StartLongRunning(
                () => {
                    var server = ClientFactory.GetServer(Credentials);
                    while (!_timerCancellationToken.IsCancellationRequested && ServerScenariosInfos.Any())
                    {
                        for (var i = 0; i < ServerScenariosInfos.Count; i++)
                        {
                            if (i >= ServerScenariosInfos.Count)
                                i = 0;

                            if (!ServerScenariosInfos.Any() || _timerCancellationToken.IsCancellationRequested)
                                break;

                            var info = ServerScenariosInfos[i];
                            var error = false;
                            Log.DebugFormat("Remote scenario refresh iteration begin: [{0}]; remote id: [{1}]", info.Name, info.ScenarioId);
                            HandleExceptions(
                                () => {
                                    var newScenInfo = server.GetScenarioInfo(new Encrypted<string>(info.ScenarioId, Credentials.SecretKey)).Decrypt(Credentials.SecretKey);
                                    if (!info.Unregistered)
                                        info.ValueChangedCallback(new RemoteScenarioValueChangedArgs(info, newScenInfo));
                                },
                                () => {
                                    error = true;
                                    if (!info.Unregistered)
                                        info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, false));
                                },
                                info);
                            Log.DebugFormat("Remote scenario refresh iteration end: [{0}]; remote id: [{1}]", info.Name, info.ScenarioId);

                            if (!ServerScenariosInfos.Any() || _timerCancellationToken.IsCancellationRequested)
                                break;

                            var timeout = error ? CalculateTimeoutOnError() : CalculateTimeout();
                            SystemUtils.Sleep(timeout, _timerCancellationToken.Token);
                        }
                    }
                    StopListen();
                },
                (e) => Log.Error("Ошибка во время выполнения прослушивания удаленных сценариев", e));
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
            ClientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged;
            StopListen();
        }

        private bool HandleExceptions(Action action, Action onException, RemoteScenarioInfo info)
        {
            var strErrPrefix = "Ошибка во время соединения с удаленным сценарием";
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                //crutch
                if (e is AggregatedCommunicationException)
                {
                    Log.InfoFormat(strErrPrefix + ". {0}; [{2}], удаленное ID:[{1}], [{3}]",
                        e.Message, info.Name, info.ScenarioId, Credentials.GetAddress());
                }
                else if (
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectNotFound) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.DecryptionError) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.ObjectAccessDenied) ||
                    SystemUtils.IsFaultExceptionHasCode(e, ServiceFaultCodes.AccessDenied))
                {
                    Log.InfoFormat(strErrPrefix + ". " + e.Message + "; [{0}], [{2}], удаленное ID:[{1}], [{3}]",
                        info.Name, info.ScenarioId, Credentials.GetAddress(), e.InnerException?.Message);
                }
                else
                {
                    Log.WarnFormat(e, strErrPrefix + ". Unrecognized exception; [{0}], [{2}], удаленное ID:[{1}], [{3}]",
                        info.Name, info.ScenarioId, Credentials.GetAddress(), e.InnerException?.Message);
                }
                onException?.Invoke();
                return false;
            }
        }
    }
}
