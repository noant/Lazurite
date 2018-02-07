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
    public static partial class RemoteScenarioChangesListener
    {
        private class ServerClientThreading: IDisposable
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
                _isListening = false;
            }

            private void StartListen() {
                _isListening = true;
                TaskUtils.StartLongRunning(
                    () => {
                        var server = ClientFactory.GetServer(Credentials);
                        while (!_timerCancellationToken.IsCancellationRequested)
                        {
                            foreach (var info in ServerScenariosInfos.ToArray())
                            {
                                if (!ServerScenariosInfos.Contains(info))
                                    continue;
                                if (_timerCancellationToken.IsCancellationRequested)
                                    break;

                                HandleExceptions(
                                    () => {
                                        var newScenInfo = server.GetScenarioInfo(new Encrypted<string>(info.ScenarioId, Credentials.SecretKey)).Decrypt(Credentials.SecretKey);
                                        info.ValueChangedCallback(new RemoteScenarioValueChangedArgs(info, newScenInfo));
                                    },
                                    () => {
                                        info.IsAvailableChangedCallback(new RemoteScenarioAvailabilityChangedArgs(info, false));
                                    },
                                    info);

                                SystemUtils.Sleep(CalculateTimeout(), _timerCancellationToken.Token);
                            }
                        }
                        _isListening = false;
                    },
                    (e) => Log.Error("Ошибка во время выполнения прослушивания удаленных сценариев", e));
            }

            private int CalculateTimeout()
            {
                const int minTimeoutMilliseconds = 1000;
                const int maxTimeoutMilliseconds = 7000;
                return minTimeoutMilliseconds + ((maxTimeoutMilliseconds - minTimeoutMilliseconds) / ServerScenariosInfos.Count);
            }

            public void Dispose()
            {
                ClientFactory.ConnectionStateChanged -= ClientFactory_ConnectionStateChanged;
                StopListen();
            }

            private bool HandleExceptions(Action action, Action onException, RemoteScenarioInfo info)
            {
                var strErrPrefix = "Error while remote scenario connection";
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
}
