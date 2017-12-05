using Lazurite.Shared;

namespace Lazurite.MainDomain
{
    public interface IClientFactory
    {
        IServer GetServer(ConnectionCredentials credentials);
        event EventsHandler<bool> ConnectionStateChanged;
    }

    public class ConnectionStateChangedEventArgs : EventsArgs<bool>
    {
        public ConnectionStateChangedEventArgs(IServer server, bool isConnected):
            base(isConnected)
        {
            Server = server;
        }

        public IServer Server { get; private set; }
    }
}
