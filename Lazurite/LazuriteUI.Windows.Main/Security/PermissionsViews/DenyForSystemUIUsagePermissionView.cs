using Lazurite.Security.Permissions;

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