using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;

namespace LazuriteMobile.App
{
    public class GeolocationDataHandler: IAddictionalDataHandler, IDisposable
    {
        private static readonly double GeolocationMetersMinimumDistance = GlobalSettings.Get(10.0);
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private Geolocation _lastLocation = Geolocation.Empty;

        private bool IsLocationAvailable() => CrossGeolocator.IsSupported;

        public void Handle(AddictionalData data, object tag)
        {
            //do nothing
        }

        public void Prepare(AddictionalData data, object tag)
        {
            data.Set(_lastLocation);
        }

        public void Initialize()
        {
            if (IsLocationAvailable())
            {
                try
                {
                    CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
                    CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                    CrossGeolocator.Current.GetLastKnownLocationAsync().ContinueWith((t) =>
                    {
                        if (t.Result != null)
                            _lastLocation = new Geolocation(t.Result.Latitude, t.Result.Longitude, t.Result.Source == LocationSource.GPS);
                    });
                    CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMinutes(10)).ContinueWith((t) =>
                    {
                        if (t.Result != null)
                            _lastLocation = new Geolocation(t.Result.Latitude, t.Result.Longitude, t.Result.Source == LocationSource.GPS);
                    });
                    CrossGeolocator.Current.StartListeningAsync(
                        minimumTime: TimeSpan.FromMinutes(2),
                        minimumDistance: GeolocationMetersMinimumDistance,
                        listenerSettings: new ListenerSettings()
                        {
                            ActivityType = ActivityType.Fitness
                        });
                }
                catch (Exception e)
                {
                    Log.Error("Error while initializing GeolocationDataHandler", e);
                }
            }
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            _lastLocation = new Geolocation(e.Position.Latitude, e.Position.Longitude, e.Position.Source == LocationSource.GPS);
        }

        public void Dispose()
        {
            if (IsLocationAvailable())
            {
                CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
                CrossGeolocator.Current.StopListeningAsync();
            }
        }
    }
}
