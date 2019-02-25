using Lazurite.Shared;

namespace LazuriteMobile.MainDomain
{
    public interface IGeolocationListener
    {
        GeolocationListenerSettings ListenerSettings { get; set; }

        int AccuracyMeters { get; set; }

        void StartListenChanges();

        void Stop();

        Geolocation LastGeolocation { get; }
    }
}