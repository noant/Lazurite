using Lazurite.MainDomain;

namespace Lazurite.Security
{
    public class GeolocationDataHandler : IAddictionalDataHandler
    {
        public void Handle(AddictionalData data)
        {
            var user = data.Resolve<User>();
            var device = data.Resolve<DeviceInfo>();
            var geolocation = data.Resolve<Geolocation>();
            if (user != null && geolocation != null)
                user.UpdateLocation(new GeolocationInfo(geolocation, device?.Name ?? "[unknown device]"));
        }

        public void Initialize()
        {
            //do nothing
        }

        public void Prepare(AddictionalData data)
        {
            //do nothing
        }
    }
}
