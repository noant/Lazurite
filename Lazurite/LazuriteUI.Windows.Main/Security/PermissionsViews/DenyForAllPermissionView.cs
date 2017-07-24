using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForAll))]
    public class DenyForAllPermissionView: PermissionViewBase
    {
        public DenyForAllPermissionView(DenyForAll permission): base(permission)
        {
            //do nothing
        }

        public override bool IsSelectButtonVisible
        {
            get
            {
                return false;
            }
        }
    }
}
