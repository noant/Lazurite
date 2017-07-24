using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
