using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Shared
{
    public interface IUsersDataAccess
    {
        void SetNeedUsers(Func<IGeolocationTarget[]> needUsers);
    }
}
