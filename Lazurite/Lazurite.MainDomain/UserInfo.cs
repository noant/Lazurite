using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class UserInfo
    {
        private List<GeolocationInfo> _locations = new List<GeolocationInfo>();

        public UserInfo(UserBase user) {
            this.Id = user.Id;
            this.Name = user.Name;
        }

        public string Name { get; private set; }
        public string Id { get; private set; }
        public GeolocationInfo[] Geolocations {
            get {
                return _locations.ToArray();
            }
        }
        
        internal void AddGeolocationIfNotLast(GeolocationInfo geolocationInfo)
        {
            var lastLocation = _locations.LastOrDefault(x => Object.ReferenceEquals(geolocationInfo.Device, x.Device));
            if (lastLocation == null || !lastLocation.Geolocation.Equals(geolocationInfo.Geolocation))
                _locations.Add(geolocationInfo);
        }
    }
}
