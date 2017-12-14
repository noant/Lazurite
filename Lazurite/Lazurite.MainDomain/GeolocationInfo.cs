using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
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
