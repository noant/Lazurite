using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.MainDomain
{
    public class UserBase: IGeolocationTarget, IMessageTarget
    {
        public string Id { get; set; } //guid
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; }

        public override string ToString() => string.Format("{0} ({1})", Name, Login);

        private List<Message> _messages = new List<Message>();
        private List<GeolocationInfo> _locations = new List<GeolocationInfo>();
        public GeolocationInfo[] Geolocations => _locations.ToArray();
        
        private void AddGeolocationIfNotLast(GeolocationInfo geolocationInfo)
        {
            var lastLocation = _locations.LastOrDefault(x => ReferenceEquals(geolocationInfo.Device, x.Device));
            if (lastLocation == null || !lastLocation.Geolocation.Equals(geolocationInfo.Geolocation))
                _locations.Add(geolocationInfo);
        }

        public void UpdateLocation(GeolocationInfo geolocationInfo) => AddGeolocationIfNotLast(geolocationInfo);

        public void SetMessage(string message, string title) => 
            _messages.Add(new Message() { DateTime = DateTime.Now, Text = message, Header = title });

        public Message[] ExtractMessages()
        {
            if (!_messages.Any())
                return null;
            var messages = _messages.ToArray();
            _messages.Clear();
            return messages;
        }
    }
}