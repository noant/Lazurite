using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class Geolocation
    {
        public static Geolocation Empty { get; private set; } = new Geolocation(double.NaN, double.NaN);

        public Geolocation(double latitude, double longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }

        public Geolocation()
        {
            //stub
        }

        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longtitude { get; set; }

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
