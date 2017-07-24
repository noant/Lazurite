using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    public static class PermissionViewsResolver
    {
        private static Dictionary<Type, Type> PermissionTypesCache = new Dictionary<Type, Type>();

        private static Type GetViewTypeByPermissionType(Type permissionType)
        {
            if (PermissionTypesCache.ContainsKey(permissionType))
                return PermissionTypesCache[permissionType];
            else {
                var type = Assembly
                    .GetCallingAssembly()
                    .GetTypes()
                    .Where(x => typeof(PermissionViewBase).IsAssignableFrom(x))
                    .Single(x => {
                        var attr = x.GetCustomAttribute<PermissionTypeAttribute>();
                        return attr != null && attr.TargetPermission.Equals(permissionType);
                    });
                PermissionTypesCache.Add(permissionType, type);
                return type;
            }
        }

        public static PermissionViewBase CreateControlBy(IPermission permission)
        {
            var type = GetViewTypeByPermissionType(permission.GetType());
            var control = type.GetConstructor(new[] { permission.GetType() }).Invoke(new[] { permission });
            return (PermissionViewBase)control;
        }
    }
}
