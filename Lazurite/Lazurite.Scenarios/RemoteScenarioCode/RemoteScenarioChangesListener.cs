using Lazurite.MainDomain;
using System.Collections.Generic;

namespace Lazurite.Scenarios.RemoteScenarioCode
{
    internal static partial class RemoteScenarioChangesListener
    {
        private static Dictionary<ConnectionCredentials, ServerClientThreading> _threadings = new Dictionary<ConnectionCredentials, ServerClientThreading>();

        public static void Register(RemoteScenarioInfo info)
        {
            lock (_threadings)
                if (!_threadings.ContainsKey(info.Credentials))
                    _threadings.Add(info.Credentials, new ServerClientThreading(info.Credentials));
            _threadings[info.Credentials].Append(info);
        }

        public static void Unregister(RemoteScenarioInfo info)
        {
            lock (_threadings)
                if (_threadings.ContainsKey(info.Credentials))
                {
                    var threading = _threadings[info.Credentials];
                    threading.Remove(info);
                    info.SetUnregistered();
                }
        }
    }
}
