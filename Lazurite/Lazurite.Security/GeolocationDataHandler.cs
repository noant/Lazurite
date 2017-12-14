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
            var clientInfo = data.Resolve<ClientAddictionalDataInfo>();
            var geolocation = data.Resolve<Geolocation>();
            if (clientInfo != null && geolocation != null)
                clientInfo.CurrentUser.UpdateLocation(new GeolocationInfo(geolocation, clientInfo.Device));
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
