using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForNetworkUsage))]
    public class DenyForNetworkUsagePermissionView : PermissionViewBase
    {
        public DenyForNetworkUsagePermissionView(DenyForNetworkUsage permission): base(permission)
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