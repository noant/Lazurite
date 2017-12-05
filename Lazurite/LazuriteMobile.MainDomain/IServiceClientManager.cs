using Lazurite.MainDomain;

namespace LazuriteMobile.MainDomain
{
    public interface IServiceClientManager
    {
        IServiceClient Create(ConnectionCredentials credentials);
    }
}
