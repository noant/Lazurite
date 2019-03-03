using Lazurite.Shared;

namespace LazuriteMobile.MainDomain
{
    public interface IGeolocationListener
    {
        GeolocationListenerSettings ListenerSettings { get; set; }

        int AccuracyMeters { get; set; }

        bool Started { get; }

        void StartListenChanges();

        void TryStartListenChanges();

        void Stop();

        Geolocation LastGeolocation { get; }
    }
}