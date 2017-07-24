using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForScenarioUsePermission))]
    public class DenyForScenarioUsePermissionView : PermissionViewBase
    {
        public DenyForScenarioUsePermissionView(DenyForScenarioUsePermission permission): base(permission)
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
