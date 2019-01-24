using Lazurite.Shared;

namespace LazuriteMobile.MainDomain
{
    public interface IGeolocationView
    {
        void View(Geolocation geolocation, string label);
    }
}
