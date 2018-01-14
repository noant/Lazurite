using Geolocation;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPlugin
{
    [OnlyGetValue]
    [HumanFriendlyName("Местоположение пользователя")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    public class UserInLocationAction : IAction, IUsersDataAccess, IUserInLocation
    {
        public string Caption {
            get => "Местоположение пользователя '" + GetUserName() + "'";
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        private string GetUserName()
        {
            var user = NeedUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            if (user != null)
                return user.Name;
            else
                return "[неизвестный]";
        }

        public ValueTypeBase ValueType {
            get {
                var valueType = new StateValueType();
                valueType.AcceptedValues = 
                    PlacesManager.Current.Places
                    .Select(x => x.Name)
                    .Union(new[] { GeolocationPlace.Other.Name, GeolocationPlace.Empty.Name })
                    .Distinct()
                    .ToArray();
                return valueType;
            }
            set { }
        }

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public Func<IGeolocationTarget[]> NeedUsers { get; set; }

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            var user = NeedUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(this.UserId));
            var userLocations = user.Geolocations.Where(x => x.Device.Equals(DeviceId)).ToArray();
            if (user == null || !userLocations.Any())
                return GeolocationPlace.Empty.Name;
            var userLastLocation = userLocations.Last();
            var places = PlacesManager.Current.Places.OrderBy(x => x.MetersRadius).ToArray();
            var targetPlace = places.Where(x =>
                GeoCalculator.GetDistance(
                    x.Location.Latitude, x.Location.Longtitude,
                    userLastLocation.Geolocation.Latitude, userLastLocation.Geolocation.Longtitude,
                    1,
                    DistanceUnit.Meters) <= x.MetersRadius)
                    .OrderBy(x => x.MetersRadius)
                    .FirstOrDefault();
            if (targetPlace == null)
                return GeolocationPlace.Other.Name;
            return targetPlace.Name;
        }

        public void Initialize()
        {
            //
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            throw new NotImplementedException();
        }
    }
}
