using System;

namespace Lazurite.Shared
{
    public interface IUsersGeolocationAccess
    {
        void SetNeedTargets(Func<IGeolocationTarget[]> needUsers);
    }
}
