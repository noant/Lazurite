using Geolocation;
using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPlugin
{
    public static class Utils
    {
        public static GeolocationPlace[] GetCurrentUserGeolocations(IGeolocationTarget[] users, string userId, string deviceName)
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user == null)
                return new GeolocationPlace[0];
            var userLocations = user.Geolocations.Where(x => x.Device.Equals(deviceName)).ToArray();
            if (!userLocations.Any())
                return new GeolocationPlace[0];
            var userLastLocation = userLocations.Last();
            var places = PlacesManager.Current.Places.OrderBy(x => x.MetersRadious).ToArray();
            var targetPlaces = places.Where(x =>
                GeoCalculator.GetDistance(
                    x.Location.Latitude, x.Location.Longtitude,
                    userLastLocation.Geolocation.Latitude, userLastLocation.Geolocation.Longtitude,
                    1,
                    DistanceUnit.Meters) <= x.MetersRadious)
                    .OrderBy(x => x.MetersRadious)
                    .ToArray();
            return targetPlaces;
        }
    }
}
