using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Shared;

namespace Lazurite.Security
{
    public class GeolocationDataHandler : IAddictionalDataHandler
    {
        private static ILogger Log = Singleton.Resolve<ILogger>();

        public void Handle(AddictionalData data)
        {
            var user = data.Resolve<User>();
            var device = data.Resolve<DeviceInfo>();
            var geolocation = data.Resolve<Geolocation>();
            if (user != null && geolocation != null)
            {
                Log.InfoFormat("User [{0}] device: [{1}];", user.Name, device?.Name ?? "unknown");
                Log.InfoFormat("User [{0}] new geolocation: [{1}]; source: [{2}];", user.Name, geolocation, geolocation.IsGPS ? "GPS" : "unknown");
                user.UpdateLocation(new GeolocationInfo(geolocation, device?.Name ?? "[unknown device]"));
            }
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
