using Lazurite.Shared;

namespace LazuriteMobile.MainDomain
{
    public interface IGeolocationListener
    {
        void StartListenChanges();
        void Stop();
        Geolocation LastGeolocation { get; } 
    }
}
