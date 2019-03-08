using Lazurite.IOC;
using Lazurite.Logging;
using LazuriteMobile.MainDomain;
using System;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public static class Helper
    {
        public static async Task TryGrantRequiredPermissions()
        {
            var log = Singleton.Resolve<ILogger>();

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

                if (!permissionsHandler.IsPermissionGranted(permissionsHandler.StartForegroundServicePermissionsIds))
                {
                    await permissionsHandler.TryGrantPermissions(permissionsHandler.StartForegroundServicePermissionsIds);
                }
            }
            catch (Exception e)
            {
                log.Error("Error while permisions request", e);
            }
        }
    }
}