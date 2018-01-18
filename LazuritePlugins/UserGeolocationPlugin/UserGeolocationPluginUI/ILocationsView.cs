using GMap.NET;
using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    public interface ILocationsView
    {
        Geolocation CurrentLocation { get; }

        void FitToMarkers();

        void NavigateTo(string userName, string deviceId);

        void NavigateTo(GeolocationPlace place);

        void RefreshWith(IGeolocationTarget[] viewTargets, GeolocationPlace[] geolocationPlaces);

        void Refresh();

        void HideDevicesExcept(string device);

        event EventsHandler<string> PlaceNavigated;

        event EventsHandler<UserAndDevice> UserNavigated;
    }
    
    public struct UserAndDevice
    {
        public UserAndDevice(string userName, string userId, string deviceId)
        {
            UserId = userId;
            UserName = userName;
            DeviceId = deviceId;
        }

        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string DeviceId { get; private set; }
    }

    public struct Place
    {
        public Place(string name)
        {
            PlaceName = name;
        }

        public string PlaceName { get; private set; }
    }

    public struct PointDate
    {
        public PointLatLng Point { get; set; }
        public DateTime DateTime { get; set; }
    }
}
