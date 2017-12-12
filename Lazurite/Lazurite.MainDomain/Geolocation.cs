using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class Geolocation
    {
        public Geolocation(double latitude, double longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }

        public double Latitude { get; private set; }
        public double Longtitude { get; private set; }
    }
}
