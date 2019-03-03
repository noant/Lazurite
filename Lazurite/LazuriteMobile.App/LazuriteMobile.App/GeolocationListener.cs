using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;
using ListenerSettings = Plugin.Geolocator.Abstractions.ListenerSettings;

namespace LazuriteMobile.App
{
    public class GeolocationListener : IGeolocationListener
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly StoredPropertiesManager PropertiesManager = Singleton.Resolve<StoredPropertiesManager>();
        private static readonly string MinimumAccuracyKey = "GeolocationMinimumAccuracy";

        private bool _started = false;
        private readonly object _locker = new object();

        private bool IsLocationAvailable() => CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled;

        public Geolocation LastGeolocation { get; private set; } = Geolocation.Empty;

        public GeolocationListenerSettings ListenerSettings
        {
            get => PropertiesManager.Get(nameof(GeolocationListenerSettings), GeolocationListenerSettings.Normal);
            set
            {
                PropertiesManager.Set(nameof(GeolocationListenerSettings), value);
                lock (_locker)
                {
                    Stop();
                    StartListenChanges();
                }
            }
        }

        public int AccuracyMeters
        {
            get => PropertiesManager.Get(MinimumAccuracyKey, 100);
            set => PropertiesManager.Set(MinimumAccuracyKey, value);
        }

        public async void StartListenChanges()
        {
            if (!CrossGeolocator.Current.IsListening &&
                !_started &&
                IsLocationAvailable() &&
                ListenerSettings.MillisecondsInterval > 0)
            {
                await StartInternal();
            }
        }

        private async Task StartInternal()
        {
            _started = true;
            try
            {
                CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
                CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                CrossGeolocator.Current.PositionError -= Current_PositionError;
                CrossGeolocator.Current.PositionError += Current_PositionError;

                var lastLocation =
                    await CrossGeolocator.Current.GetLastKnownLocationAsync() ??
                    await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMinutes(10));

                if (lastLocation != null && lastLocation.Accuracy <= AccuracyMeters)
                {
                    LastGeolocation = new Geolocation(lastLocation.Latitude, lastLocation.Longitude, true);
                }

                var startListenGeolocation =
                    CrossGeolocator.Current.StartListeningAsync(
                        minimumTime: TimeSpan.FromMilliseconds(ListenerSettings.MillisecondsInterval),
                        minimumDistance: ListenerSettings.MetersInterval,
                        listenerSettings: new ListenerSettings()
                        {
                            ActivityType = ActivityType.Other,
                            AllowBackgroundUpdates = true
                        });

                if (!await startListenGeolocation)
                {
                    throw new InvalidOperationException("Cannot start geolocation listener.");
                }
            }
            catch (Exception e)
            {
                _started = false;
                Log.Error("Error while initializing GeolocationDataHandler", e);
            }
        }

        public void Stop() => StopInternal().Wait();

        private async Task StopInternal()
        {
            _started = false;
            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;
            CrossGeolocator.Current.PositionError -= Current_PositionError;
            await CrossGeolocator.Current.StopListeningAsync();
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            if (e.Position.Accuracy <= AccuracyMeters)
            {
                LastGeolocation = new Geolocation(e.Position.Latitude, e.Position.Longitude, true);
            }
        }

        private async void Current_PositionError(object sender, PositionErrorEventArgs e)
        {
            await StopInternal();
            await StartInternal();
        }
    }
}