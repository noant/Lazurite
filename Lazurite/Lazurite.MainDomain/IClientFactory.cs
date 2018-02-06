using Lazurite.Shared;

namespace Lazurite.MainDomain
{
    public interface IClientFactory
    {
        IServer GetServer(ConnectionCredentials credentials);

        ConnectionCredentials[] ConnectionCredentials { get; }

        event EventsHandler<bool> ConnectionStateChanged;
    }

    public class ConnectionStateChangedEventArgs : EventsArgs<bool>
    {
        public ConnectionStateChangedEventArgs(IServer server, bool isConnected, ConnectionCredentials credentials):
            base(isConnected)
        {
            Server = server;
            Credentials = credentials;
        }

        public IServer Server { get; }

        public ConnectionCredentials Credentials { get; }
    }
}
