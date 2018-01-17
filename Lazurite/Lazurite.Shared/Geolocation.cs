using System.Runtime.Serialization;

namespace Lazurite.Shared
{
    [DataContract]
    public class Geolocation
    {
        public static Geolocation Empty { get; private set; } = new Geolocation(double.NaN, double.NaN, false);

        public Geolocation(double latitude, double longtitude, bool isGps)
        {
            Latitude = latitude;
            Longtitude = longtitude;
            IsGPS = isGps;
        }

        public Geolocation()
        {
            //stub
        }

        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longtitude { get; set; }
        [DataMember]
        public bool IsGPS { get; set; }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longtitude.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Geolocation && GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}", Latitude.ToString().Replace(",", "."), Longtitude.ToString().Replace(",", "."));
        }
    }
}
