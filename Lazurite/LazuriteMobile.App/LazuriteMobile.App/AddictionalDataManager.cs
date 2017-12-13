using Lazurite.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;

namespace LazuriteMobile.App
{
    public class AddictionalDataManager
    {
        private static Geolocation LastGeolocation;

        static AddictionalDataManager()
        {
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            LastGeolocation = new Geolocation(-1, -1);
            CrossGeolocator.Current.GetLastKnownLocationAsync().ContinueWith((t) => {
                if (t.Result != null)
                    LastGeolocation = new Geolocation(t.Result.Latitude, t.Result.Longitude);
            });
            CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMinutes(10)).ContinueWith((t) => {
                if (t.Result != null)
                    LastGeolocation = new Geolocation(t.Result.Latitude, t.Result.Longitude);
            });
        }

        private static void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            LastGeolocation = new Geolocation(e.Position.Latitude, e.Position.Longitude);
        }

        private bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        private Geolocation GetGeolocation()
        {
            return LastGeolocation;
        }

        public AddictionalData Prepare()
        {
            var data = new AddictionalData();
            if (LastGeolocation != null)
                data.Set(LastGeolocation);
            return data;
        }

        public void Handle(AddictionalData data)
        {
            //fo future
        }
    }
}
