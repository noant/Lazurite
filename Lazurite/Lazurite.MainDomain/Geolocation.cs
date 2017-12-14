using System.Runtime.Serialization;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class Geolocation
    {
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
    }
}
