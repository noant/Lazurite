using Lazurite.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;

namespace LazuriteMobile.App
{
    public class GeolocationDataHandler: IAddictionalDataHandler
    {
        private static readonly double GeolocationMetersMinimumDistance = GlobalSettings.Get(10.0);
        private static readonly int GeolocationMinutesMinimumInterval = GlobalSettings.Get(5);
        private Geolocation _lastLocation;
        
        private bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public void Handle(AddictionalData data)
        {
            //do nothing
        }

        public void Prepare(AddictionalData data)
        {
            if (IsLocationAvailable() && _lastLocation != null)
                data.Set(_lastLocation);
        }

        public void Initialize()
        {
            if (IsLocationAvailable)
            {
                CrossGeolocator.Current.PositionChanged += (o, e) => _lastLocation = new Geolocation(e.Position.Latitude, e.Position.Longitude);
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
                    minimumTime: TimeSpan.FromMinutes(GeolocationMinutesMinimumInterval),
                    minimumDistance: GeolocationMetersMinimumDistance,
                    listenerSettings: new ListenerSettings()
                    {
                        ActivityType = ActivityType.Other
                    });
            }
        }
    }
}
