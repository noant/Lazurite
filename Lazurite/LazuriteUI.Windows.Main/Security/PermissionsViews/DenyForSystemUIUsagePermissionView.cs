using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForSystemUIUsage))]
    public class DenyForSystemUIUsagePermissionView : PermissionViewBase
    {
        public DenyForSystemUIUsagePermissionView(DenyForSystemUIUsage permission): base(permission)
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