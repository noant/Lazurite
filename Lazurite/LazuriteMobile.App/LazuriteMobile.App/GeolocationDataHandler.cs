using Lazurite.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;

namespace LazuriteMobile.App
{
    public class GeolocationDataHandler: IAddictionalDataHandler, IDisposable
    {
        private static readonly double GeolocationMetersMinimumDistance = GlobalSettings.Get(10.0);
        private Geolocation _lastLocation;

        private bool IsLocationAvailable() => CrossGeolocator.IsSupported;

        public void Handle(AddictionalData data)
        {
            //do nothing
        }

        public void Prepare(AddictionalData data)
        {
            if (_lastLocation != null)
                data.Set(_lastLocation);
        }

        public void Initialize()
        {
            if (IsLocationAvailable())
            {
                CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                CrossGeolocator.Current.GetLastKnownLocationAsync().ContinueWith((t) =>
                {
                    if (t.Result != null)
                        _lastLocation = new Geolocation(t.Result.Latitude, t.Result.Longitude);
                });
                CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMinutes(10)).ContinueWith((t) =>
                {
                    if (t.Result != null)
                        _lastLocation = new Geolocation(t.Result.Latitude, t.Result.Longitude);
                });
                CrossGeolocator.Current.StartListeningAsync(
                    minimumTime: TimeSpan.FromMinutes(2),
                    minimumDistance: GeolocationMetersMinimumDistance,
                    listenerSettings: new ListenerSettings()
                    {
                        ActivityType = ActivityType.AutomotiveNavigation
                    });
            }
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            _lastLocation = new Geolocation(e.Position.Latitude, e.Position.Longitude);
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
