using Lazurite.Shared;
using SimpleRemoteMethods.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lazurite.MainDomain
{
    public class ServiceClientFactory
    {
        public static ServiceClientFactory Current { get; } = new ServiceClientFactory();

        private static readonly double ConnectionTimeout_Minutes = GlobalSettings.Get(1.0d);

        private Dictionary<ConnectionCredentials, LazuriteClient> _clients = new Dictionary<ConnectionCredentials, LazuriteClient>();

        public ConnectionCredentials[] ConnectionCredentials => _clients.Keys.ToArray();

        public LazuriteClient GetClient(ConnectionCredentials credentials)
        {
            LazuriteClient @object;

            lock (_clients)
            {
                if (!_clients.ContainsKey(credentials))
                    _clients.Add(credentials, @object = CreateClient(credentials));
                else
                    @object = _clients[credentials];
            }

            return @object;
        }

        private LazuriteClient CreateClient(ConnectionCredentials credentials)
        {
            var client = new LazuriteClient(
                credentials.Host,
                credentials.Port,
                true,
                credentials.SecretKey,
                credentials.Login,
                credentials.Password,
                TimeSpan.FromMinutes(ConnectionTimeout_Minutes));

            client.Client.ConnectionError += (o, e) =>
            {
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(client, false, credentials));
                Debug.WriteLine(e);
            };

            client.Client.ConnectionNormal += (o, e) =>
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(client, true, credentials));

            return client;
        }

        public event EventsHandler<bool> ConnectionStateChanged;

        public class ConnectionStateChangedEventArgs : EventsArgs<bool>
        {
            public ConnectionStateChangedEventArgs(LazuriteClient client, bool isConnected, ConnectionCredentials credentials) :
                base(isConnected)
            {
                Client = client;
                Credentials = credentials;
            }

            public LazuriteClient Client { get; }

            public ConnectionCredentials Credentials { get; }
        }
    }
}
