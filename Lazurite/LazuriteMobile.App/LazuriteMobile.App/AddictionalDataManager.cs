using Lazurite.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class AddictionalDataManager
    {
        private static Geolocation LastGeolocation;

        static AddictionalDataManager()
        {
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            CrossGeolocator.Current.GetPositionAsync().ContinueWith((t) =>
                {
                    if (t.Result != null)
                        LastGeolocation = new Geolocation(t.Result.Latitude, t.Result.Longitude);
                }
            );
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
            var data1 = data;
        }
    }
}
