using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPlugin
{
    public interface IUserInLocation
    {
        string UserId { get; set; }
        string DeviceId { get; set; }
    }
}
