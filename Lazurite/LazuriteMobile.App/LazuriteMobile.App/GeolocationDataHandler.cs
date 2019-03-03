using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App
{
    public class GeolocationDataHandler : IAddictionalDataHandler
    {
        private static readonly IGeolocationListener Listener = Singleton.Resolve<IGeolocationListener>();

        public void Handle(AddictionalData data, object tag)
        {
            // Do nothing
        }

        public void Prepare(AddictionalData data, object tag)
        {
            data.Set(Listener.LastGeolocation);
        }

        public void Initialize()
        {
            // Do nothing
        }
    }
}