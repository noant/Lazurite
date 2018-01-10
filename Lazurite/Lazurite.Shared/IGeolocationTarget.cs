using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Shared
{
    public interface IGeolocationTarget
    {
        string Name { get; }
        string Id { get; }
        GeolocationInfo[] Geolocations { get; }
    }
}
