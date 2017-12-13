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
