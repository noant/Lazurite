namespace Lazurite.Shared
{
    public interface IGeolocationTarget
    {
        string Name { get; }
        string Id { get; }
        GeolocationInfo[] Geolocations { get; }
    }
}
