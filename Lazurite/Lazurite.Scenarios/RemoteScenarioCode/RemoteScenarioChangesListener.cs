using Lazurite.MainDomain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    _threadings[info.Credentials].Remove(info);
                    info.SetUnregistered();
                }
        }
    }
}
