using Lazurite.Security.Permissions;

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