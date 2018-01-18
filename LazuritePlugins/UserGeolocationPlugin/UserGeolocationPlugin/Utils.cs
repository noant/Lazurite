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
        public static readonly double EquatorLength = 40075696.0;

        private static GeolocationInfo[] GetNonGpsLocations(IGeolocationTarget target, string device)
        {
            var userLocations = target.Geolocations.Where(x => x.Device.Equals(device)).ToArray(); //take first even it source is not GPS
            return userLocations.Where(x => x.Geolocation.IsGPS || x == userLocations[0]).ToArray();
        }

        public static GeolocationPlace[] GetUserCurrentLocations(IGeolocationTarget[] users, string userId, string deviceName)
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user == null)
                return new GeolocationPlace[] { GeolocationPlace.Empty };
            var userLocations = GetNonGpsLocations(user, deviceName);
            if (!userLocations.Any())
                return new GeolocationPlace[] { GeolocationPlace.Other };
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
            return targetPlaces.Any() ? targetPlaces : new GeolocationPlace[] { GeolocationPlace.Other };
        }

        public static GeolocationPlace GetCurrentUserLocation(IGeolocationTarget[] users, string userId, string deviceName)
        {
            var locations = GetUserCurrentLocations(users, userId, deviceName);
            return locations.FirstOrDefault();
        }

        public static Lazurite.Shared.Geolocation GetUserCurrentGeolocation(IGeolocationTarget[] users, string userId, string deviceName)
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user == null)
                return Lazurite.Shared.Geolocation.Empty;
            var locationsInfos = GetNonGpsLocations(user, deviceName);
            return locationsInfos.LastOrDefault()?.Geolocation ?? Lazurite.Shared.Geolocation.Empty;
        }
        
        public static double GetDistanceBetween(IGeolocationTarget[] users, string userId, string device, string placeName)
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user == null)
                return EquatorLength;
            var place = PlacesManager.GetAllAvailablePlaces().FirstOrDefault(x => x.Name.Equals(placeName));
            if (place == null || place == GeolocationPlace.Empty || place == GeolocationPlace.Other)
                return EquatorLength;
            var userCurrentGeolocation = GetUserCurrentGeolocation(users, userId, device);
            if (userCurrentGeolocation == Lazurite.Shared.Geolocation.Empty)
                return EquatorLength;
            return GeoCalculator.GetDistance(
                userCurrentGeolocation.Latitude, userCurrentGeolocation.Longtitude,
                place.Location.Latitude, place.Location.Longtitude, 1, DistanceUnit.Meters);
        }
    }
}
