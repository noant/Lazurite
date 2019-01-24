using System;

namespace Lazurite.Shared
{
    public class GeolocationInfo
    {
        public DateTime DateTime { get; set; }
        public Geolocation Geolocation { get; private set; }
        public string Device { get; set; }

        public GeolocationInfo(Geolocation location, string device)
        {
            DateTime = DateTime.Now;
            Geolocation = location;
            Device = device;
        }
    }
}
