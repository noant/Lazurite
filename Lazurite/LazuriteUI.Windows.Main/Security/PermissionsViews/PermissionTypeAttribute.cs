using System;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    public class PermissionTypeAttribute: Attribute
    {
        public PermissionTypeAttribute(Type permissionType)
        {
            TargetPermission = permissionType;
        }
        public Type TargetPermission { get; private set; }
    }
}
