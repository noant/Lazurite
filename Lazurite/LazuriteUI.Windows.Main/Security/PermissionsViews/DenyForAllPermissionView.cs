using Lazurite.Security.Permissions;

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
