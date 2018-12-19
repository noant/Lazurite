using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;
using System.Net;
using System.ServiceModel;

namespace LazuriteMobile.MainDomain
{
    public interface IClientManager
    {
        void CreateConnection(ConnectionCredentials credentials);
        IServer GetActualInstance();
        bool IsClosed();
        void Close();
        void Abort(IServer client);
        void Close(IServer client);
    }
}