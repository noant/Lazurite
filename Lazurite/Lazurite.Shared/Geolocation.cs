using ProtoBuf;

namespace Lazurite.Shared
{
    [ProtoContract]
    public class Geolocation
    {
        public static readonly Geolocation Empty = new Geolocation(double.NaN, double.NaN, false);

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

        [ProtoMember(1)]
        public double Latitude { get; set; } = double.NaN;
        [ProtoMember(2)]
        public double Longtitude { get; set; } = double.NaN;
        [ProtoMember(3)]
        public bool IsGPS { get; set; }

        public bool IsInvalid => Equals(Empty);

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
