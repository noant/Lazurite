using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                user.UpdateLocation(new GeolocationInfo(geolocation, device.Name));
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
