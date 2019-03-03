using System.Collections.Generic;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public interface IRuntimePermissionsHandler
    {
        bool IsPermissionGranted(string[] permissionsids);

        Task<Dictionary<string, bool>> TryGrantPermissions(string[] permissionsids);

        void ResolvePermissionCallback(int code, Dictionary<string, bool> grantList);

        string[] GpsPermissionsIds { get; }

        string[] AutoStartPermissionsIds { get; }

        string[] WakeLockPermissionsIds { get; }

        string[] ReadWriteExternalStoragePermissionsIds { get; }

        string[] PhoneStatePermissionsIds { get; }
    }
}