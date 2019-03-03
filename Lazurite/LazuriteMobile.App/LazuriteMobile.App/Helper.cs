using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public static class Helper
    {
        public static async Task TryGrantRequiredPermissions()
        {
            try
            {
                var permissionsHandler = Singleton.Resolve<IRuntimePermissionsHandler>();

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.GpsPermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.GpsPermissionsIds);
                }

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.AutoStartPermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.AutoStartPermissionsIds);
                }

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.PhoneStatePermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.PhoneStatePermissionsIds);
                }

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.ReadWriteExternalStoragePermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.ReadWriteExternalStoragePermissionsIds);
                }

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.WakeLockPermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.WakeLockPermissionsIds);
                }
            }
            catch
            {
                // Do nothing
            }
        }
    }
}