using Lazurite.Security.Permissions;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Lazurite.Security
{
    public static class Utils
    {
        private static TypeInfo[] _permissionTypes;
        public static TypeInfo[] GetPermissionTypes()
        {
            if (_permissionTypes == null)
            {
                _permissionTypes = ReflectionUtils.GetAllOfType(typeof(IPermission)).ToArray();
            }
            return _permissionTypes;
        }

        public static IPermission CreateInstanceOfPermission(TypeInfo permissionType)
        {
            return (IPermission)Activator.CreateInstance(permissionType.GetType());
        }
    }
}
