using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class GeolocationListener: IGeolocationListener
    {
        private static readonly double GeolocationMetersMinimumDistance = GlobalSettings.Get(10.0);
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private bool _started = false;

        public Geolocation LastGeolocation { get; private set; } = Geolocation.Empty;

        private Task<bool> IsLocationAvailable() => CrossGeolocator.Current.CheckPermissionsAsync();

        public void StartListenChanges()
        {
            if (!_started)
            {
                _started = true;
                IsLocationAvailable().ContinueWith((isLocationAvailable) =>
                {
                    try
                    {
                        if (isLocationAvailable.Result)
                        {
                            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
                            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                            CrossGeolocator.Current.GetLastKnownLocationAsync().ContinueWith((t) =>
                            {
                                if (t.Result != null)
                                    LastGeolocation = new Geolocation(t.Result.Latitude, t.Result.Longitude, t.Result.Source == LocationSource.GPS);
                            });
                            CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMinutes(10)).ContinueWith((t) =>
                            {
                                if (t.Result != null)
                                    LastGeolocation = new Geolocation(t.Result.Latitude, t.Result.Longitude, t.Result.Source == LocationSource.GPS);
                            });
                            CrossGeolocator.Current.StartListeningAsync(
                                minimumTime: TimeSpan.FromMinutes(2),
                                minimumDistance: GeolocationMetersMinimumDistance,
                                listenerSettings: new ListenerSettings()
                                {
                                    ActivityType = ActivityType.Fitness
                                });
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error while initializing GeolocationDataHandler", e);
                    }
                });
            }
        }

        public void Stop()
        {
            _started = false;
            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
            CrossGeolocator.Current.StopListeningAsync();
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            if (e.Position.Accuracy <= 100)
                LastGeolocation = new Geolocation(e.Position.Latitude, e.Position.Longitude, e.Position.Source == LocationSource.GPS);
        }
    }
}
